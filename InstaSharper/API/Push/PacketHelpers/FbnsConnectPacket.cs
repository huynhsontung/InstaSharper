using System;
using DotNetty.Buffers;
using DotNetty.Codecs.Mqtt.Packets;

namespace InstaSharper.API.Push.PacketHelpers
{
    public class FbnsConnectPacket : Packet
    {
        public override PacketType PacketType { get; } = PacketType.CONNECT;

        /// <summary>
        ///     Following flags are marked: User Name Flag, Password Flag, Clean Session
        /// </summary>
        public int ConnectFlags { get; } = 194;

        public string ProtocolName { get; } = "MQTToT";

        public int ProtocolLevel { get; } = 3;

        private int _keepAlive = 900;

        public int KeepAliveInSeconds
        {
            get => _keepAlive;
            set
            {
                if (value > 65535) throw new ArgumentOutOfRangeException();
                _keepAlive = value;
            }
        }

        public IByteBuffer Payload { get; set; }
    }
}
