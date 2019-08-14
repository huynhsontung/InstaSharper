using System.Data;
using DotNetty.Buffers;
using DotNetty.Codecs.Mqtt.Packets;

namespace InstaSharper.API.Push.PacketHelpers
{
    public sealed class FbnsConnectPacket : Packet
    {
        public override PacketType PacketType { get; } = PacketType.CONNECT;

        /// <summary>
        ///     Following flags are marked: User Name Flag, Password Flag, Clean Session
        /// </summary>
        public int ConnectFlags { get; } = 194;

        public string ProtocolName { get; } = "MQTToT";

        public int ProtocolLevel { get; } = 3;

        private ushort _keepAlive = 900;

        public ushort KeepAliveInSeconds
        {
            get => _keepAlive;
            set
            {
                if (value < 60) throw new ConstraintException("Keep alive duration too short. Keep alive needs to be longer than 60 seconds");
                _keepAlive = value;
            }
        }

        public IByteBuffer Payload { get; set; }
    }
}
