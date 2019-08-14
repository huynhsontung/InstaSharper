using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using DotNetty.Buffers;
using DotNetty.Codecs;
using DotNetty.Codecs.Mqtt.Packets;
using DotNetty.Transport.Channels;

namespace InstaSharper.API.Push.PacketHelpers
{
    /// <summary>
    ///     Customized MqttDecoder for Fbns that only handles Publish, PubAck, and ConnAck
    /// </summary>
    /// Reference: https://github.com/Azure/DotNetty/blob/dev/src/DotNetty.Codecs.Mqtt/MqttDecoder.cs
    public sealed class FbnsPacketDecoder : ReplayingDecoder<FbnsPacketDecoder.ParseState>
    {
        private static class Signatures
        {
            public const byte PubAck = 64;
            public const byte ConnAck = 32;
//            public const byte PubRec = 80;
//            public const byte PubRel = 98;
//            public const byte PubComp = 112;
//            public const byte Connect = 16;
//            public const byte Subscribe = 130;
//            public const byte SubAck = 144;
//            public const byte PingReq = 192;
            public const byte PingResp = 208;
//            public const byte Disconnect = 224;
//            public const byte Unsubscribe = 162;
//            public const byte UnsubAck = 176;

            public static bool IsPublish(int signature)
            {
                return (signature & 240) == 48;
            }
        }

        public enum ParseState
        {
            Ready,
            Failed
        }

        public FbnsPacketDecoder() : base(ParseState.Ready)
        {
        }

        protected override void Decode(IChannelHandlerContext context, IByteBuffer input, List<object> output)
        {
            try
            {
                switch (this.State)
                {
                    case ParseState.Ready:
                        Packet packet;

                        if (!this.TryDecodePacket(input, out packet))
                        {
                            this.RequestReplay();
                            return;
                        }

                        output.Add(packet);
                        this.Checkpoint();
                        break;
                    case ParseState.Failed:
                        // read out data until connection is closed
                        input.SkipBytes(input.ReadableBytes);
                        return;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            catch (DecoderException)
            {
                input.SkipBytes(input.ReadableBytes);
                this.Checkpoint(ParseState.Failed);
                throw;
            }
        }

        private bool TryDecodePacket(IByteBuffer buffer, out Packet packet)
        {
            // Check fixed header
            if (!buffer.IsReadable(2)) // packet consists of at least 2 bytes
            {
                packet = null;
                return false;
            }

            int signature = buffer.ReadByte();

            int remainingLength;

            if (!this.TryDecodeRemainingLength(buffer, out remainingLength) || 
                !buffer.IsReadable(remainingLength) ||
                remainingLength < 2)
            {
                packet = null;
                return false;
            }

            packet = this.DecodePacketInternal(buffer, signature, ref remainingLength);

            if (remainingLength > 0)
            {
                throw new DecoderException($"Declared remaining length is bigger than packet data size by {remainingLength}.");
            }

            return true;
        }

        private Packet DecodePacketInternal(IByteBuffer buffer, int packetSignature, ref int remainingLength)
        {
            if (Signatures.IsPublish(packetSignature))
            {
                var qualityOfService =
                    (QualityOfService) ((packetSignature >> 1) &
                                        0x3); // take bits #1 and #2 ONLY and convert them into QoS value
                if (qualityOfService == QualityOfService.Reserved)
                {
                    throw new DecoderException(
                        $"Unexpected QoS value of {(int) qualityOfService} for {PacketType.PUBLISH} packet.");
                }

                bool duplicate = (packetSignature & 0x8) == 0x8; // test bit#3
                bool retain = (packetSignature & 0x1) != 0; // test bit#0
                var packet = new PublishPacket(qualityOfService, duplicate, retain);
                DecodePublishPacket(buffer, packet, ref remainingLength);
                return packet;
            }

            switch (packetSignature & 240)  // We don't care about flags for these packets
            {
                case Signatures.PubAck:
                    var pubAckPacket = new PubAckPacket();
                    DecodePacketIdVariableHeader(buffer, pubAckPacket, ref remainingLength);
                    return pubAckPacket;
                case Signatures.ConnAck:
                    var connAckPacket = new FbnsConnAckPacket();
                    DecodeConnAckPacket(buffer, connAckPacket, ref remainingLength);
                    return connAckPacket;
                case Signatures.PingResp:
                    return PingRespPacket.Instance;
                default:
                    throw new DecoderException($"Packet type {packetSignature} not supported");
            }
        }

        static void DecodeConnAckPacket(IByteBuffer buffer, FbnsConnAckPacket packet, ref int remainingLength)
        {
            packet.ConnAckFlags = buffer.ReadByte();
            packet.ReturnCode = (ConnectReturnCode) buffer.ReadByte();
            remainingLength -= 2;
            if (remainingLength > 0)
            {
                var authSize = buffer.ReadUnsignedShort();
                packet.Authentication = buffer.ReadString(authSize, Encoding.UTF8);
                remainingLength -= authSize + 2;
                if(remainingLength>0)
                    Debug.WriteLine(
                        $"FbnsPacketDecoder: Unhandled data in the buffer. Length of remaining data = {remainingLength}",
                        "Warning");
            }
        }

        static void DecodePublishPacket(IByteBuffer buffer, PublishPacket packet, ref int remainingLength)
        {
            string topicName = DecodeString(buffer, ref remainingLength);

            packet.TopicName = topicName;
            if (packet.QualityOfService > QualityOfService.AtMostOnce)
            {
                DecodePacketIdVariableHeader(buffer, packet, ref remainingLength);
            }

            IByteBuffer payload;
            if (remainingLength > 0)
            {
                payload = buffer.ReadSlice(remainingLength);
                payload.Retain();
                remainingLength = 0;
            }
            else
            {
                payload = Unpooled.Empty;
            }
            packet.Payload = payload;
        }


        private bool TryDecodeRemainingLength(IByteBuffer buffer, out int value)
        {
            int readable = buffer.ReadableBytes;

            int result = 0;
            int multiplier = 1;
            byte digit;
            int read = 0;
            do
            {
                if (readable < read + 1)
                {
                    value = default(int);
                    return false;
                }
                digit = buffer.ReadByte();
                result += (digit & 0x7f) * multiplier;
                multiplier <<= 7;
                read++;
            }
            while ((digit & 0x80) != 0 && read < 4);

            if (read == 4 && (digit & 0x80) != 0)
            {
                throw new DecoderException("Remaining length exceeds 4 bytes in length");
            }

            value = result;
            return true;
        }

        static void DecodePacketIdVariableHeader(IByteBuffer buffer, PacketWithId packet, ref int remainingLength)
        {
            int packetId = packet.PacketId = DecodeUnsignedShort(buffer, ref remainingLength);
            if (packetId == 0)
            {
                throw new DecoderException("[MQTT-2.3.1-1]");
            }
        }

        static int DecodeUnsignedShort(IByteBuffer buffer, ref int remainingLength)
        {
            DecreaseRemainingLength(ref remainingLength, 2);
            return buffer.ReadUnsignedShort();
        }

        static string DecodeString(IByteBuffer buffer, ref int remainingLength) => DecodeString(buffer, ref remainingLength, 0, int.MaxValue);

        static string DecodeString(IByteBuffer buffer, ref int remainingLength, int minBytes) => DecodeString(buffer, ref remainingLength, minBytes, int.MaxValue);

        static string DecodeString(IByteBuffer buffer, ref int remainingLength, int minBytes, int maxBytes)
        {
            int size = DecodeUnsignedShort(buffer, ref remainingLength);

            if (size < minBytes)
            {
                throw new DecoderException($"String value is shorter than minimum allowed {minBytes}. Advertised length: {size}");
            }
            if (size > maxBytes)
            {
                throw new DecoderException($"String value is longer than maximum allowed {maxBytes}. Advertised length: {size}");
            }

            if (size == 0)
            {
                return string.Empty;
            }

            DecreaseRemainingLength(ref remainingLength, size);

            string value = buffer.ToString(buffer.ReaderIndex, size, Encoding.UTF8);
            // todo: enforce string definition by MQTT spec
            buffer.SetReaderIndex(buffer.ReaderIndex + size);
            return value;
        }

        static void DecreaseRemainingLength(ref int remainingLength, int minExpectedLength)
        {
            if (remainingLength < minExpectedLength)
            {
                throw new DecoderException($"Current Remaining Length of {remainingLength} is smaller than expected {minExpectedLength}.");
            }
            remainingLength -= minExpectedLength;
        }

    }
}
