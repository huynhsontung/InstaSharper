using DotNetty.Codecs.Mqtt.Packets;

namespace InstaSharper.API.Push.PacketHelpers
{
    public sealed class FbnsConnAckPacket : Packet
    {
        public override PacketType PacketType { get; } = PacketType.CONNACK;

        public int ConnAckFlags { get; set; }

        public ConnectReturnCode ReturnCode { get; set; }

        public string Authentication { get; set; }
    }
}
