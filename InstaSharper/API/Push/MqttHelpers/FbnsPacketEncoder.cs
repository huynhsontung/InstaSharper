using System.Collections.Generic;
using System.Text;
using DotNetty.Buffers;
using DotNetty.Codecs;
using DotNetty.Codecs.Mqtt;
using DotNetty.Codecs.Mqtt.Packets;
using DotNetty.Common.Utilities;
using DotNetty.Transport.Channels;
using DotNetty.Transport.Channels.Embedded;

namespace InstaSharper.API.Push.MqttHelpers
{
    public sealed class FbnsPacketEncoder : MessageToMessageEncoder<Packet>
    {
        public static readonly FbnsPacketEncoder Instance = new FbnsPacketEncoder();

        private static readonly EmbeddedChannel MqttEncodeChannel = new EmbeddedChannel(new MqttEncoder());

        const int PACKET_ID_LENGTH = 2;
        const int STRING_SIZE_LENGTH = 2;
        const int MAX_VARIABLE_LENGTH = 4;

        public override bool IsSharable => true;

        protected override void Encode(IChannelHandlerContext context, Packet packet, List<object> output)
        {
            if (!(packet is FbnsConnectPacket connectPacket))
            {
                MqttEncodeChannel.WriteOutbound(packet);
                IByteBuffer result;
                do
                {
                    result = MqttEncodeChannel.ReadOutbound<IByteBuffer>();
                    if (result != null) output.Add(result);
                } while (result != null);
                return;
            }
            var bufferAllocator = context.Allocator;
            var payload = connectPacket.Payload;
            if (payload == null) throw new EncoderException("Payload required");
            int payloadSize = payload.ReadableBytes;
            byte[] protocolNameBytes = EncodeStringInUtf8(connectPacket.ProtocolName);
            // variableHeaderBufferSize = 2 bytes length + ProtocolName bytes + 4 bytes
            // 4 bytes are reserved for: 1 byte ProtocolLevel, 1 byte ConnectFlags, 2 byte KeepAlive
            int variableHeaderBufferSize = STRING_SIZE_LENGTH + protocolNameBytes.Length + 4; 
            int variablePartSize = variableHeaderBufferSize + payloadSize;
            int fixedHeaderBufferSize = 1 + MAX_VARIABLE_LENGTH;
            IByteBuffer buf = null;
            try
            {
                // MQTT message format from: http://public.dhe.ibm.com/software/dw/webservices/ws-mqtt/MQTT_V3.1_Protocol_Specific.pdf
                buf = bufferAllocator.Buffer(fixedHeaderBufferSize + variableHeaderBufferSize);
                buf.WriteByte((int)connectPacket.PacketType << 4); // Write packet type
                WriteVariableLengthInt(buf, variablePartSize); // Write remaining length

                // Variable part
                buf.WriteShort(protocolNameBytes.Length);
                buf.WriteBytes(protocolNameBytes);
                buf.WriteByte(connectPacket.ProtocolLevel);
                buf.WriteByte(connectPacket.ConnectFlags);
                buf.WriteShort(connectPacket.KeepAliveInSeconds);

                output.Add(buf);
                buf = null;
            }
            finally
            {
                buf?.SafeRelease();
            }

            if (payload.IsReadable())
            {
                output.Add(payload.Retain());
            }
        }

        static void WriteVariableLengthInt(IByteBuffer buffer, int value)
        {
            do
            {
                int digit = value % 128;
                value /= 128;
                if (value > 0)
                {
                    digit |= 0x80;
                }
                buffer.WriteByte(digit);
            }
            while (value > 0);
        }

        static byte[] EncodeStringInUtf8(string s)
        {
            return Encoding.UTF8.GetBytes(s);
        }
    }
}