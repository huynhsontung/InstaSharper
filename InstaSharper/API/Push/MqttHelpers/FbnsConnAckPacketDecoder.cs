using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using DotNetty.Buffers;
using DotNetty.Codecs;
using DotNetty.Codecs.Mqtt.Packets;
using DotNetty.Transport.Channels;

namespace InstaSharper.API.Push.MqttHelpers
{
    public sealed class FbnsConnAckPacketDecoder : ReplayingDecoder<FbnsConnAckPacketDecoder.ParseState>
    {
        private const byte CONNACK_SIGNATURE = 32;

        public enum ParseState
        {
            Ready,
            Failed
        }

        public FbnsConnAckPacketDecoder() : base(ParseState.Ready)
        {
        }

        protected override void Decode(IChannelHandlerContext context, IByteBuffer input, List<object> output)
        {
            try
            {
                switch (this.State)
                {
                    case ParseState.Ready:
                        FbnsConnAckPacket packet;

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

        private bool TryDecodePacket(IByteBuffer buffer, out FbnsConnAckPacket packet)
        {
            // Check fixed header
            if (!buffer.IsReadable(2)) // packet consists of at least 2 bytes
            {
                packet = null;
                return false;
            }

            int signature = buffer.ReadByte();
            if (signature != CONNACK_SIGNATURE)
            {
                Debug.WriteLine("Raw bytes:");
                Debug.Write(signature + " ");
                while (buffer.ReadableBytes > 0)
                {
                    Debug.Write($"{buffer.ReadByte()} ");
                }

                Debug.WriteLine("");
                packet = null;
                return false;
            }

            int remainingLength;
            if (!this.TryDecodeRemainingLength(buffer, out remainingLength) || 
                !buffer.IsReadable(remainingLength) ||
                remainingLength < 2)
            {
                packet = null;
                return false;
            }

            // Start decoding body
            packet = new FbnsConnAckPacket
            {
                ConnAckFlags = buffer.ReadByte(),
                ReturnCode = (ConnectReturnCode) buffer.ReadByte()
            };
            remainingLength -= 2;
            if (remainingLength > 0)
            {
                packet.Authentication = buffer.ReadString(remainingLength, Encoding.UTF8);
            }
            return true;
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
    }
}
