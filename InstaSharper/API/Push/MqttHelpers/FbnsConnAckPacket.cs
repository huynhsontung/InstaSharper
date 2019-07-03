using DotNetty.Codecs.Mqtt.Packets;

namespace InstaSharper.API.Push.MqttHelpers
{
    class FbnsConnAckPacket : DotNetty.Codecs.Mqtt.Packets.Packet
    {
        public override PacketType PacketType { get; } = PacketType.CONNACK;

        public int ConnAckFlags { get; set; }   // ???

        public ConnectReturnCode ReturnCode { get; set; }

        public string Authentication { get; set; }
    }
}
