using System;
using System.Diagnostics;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using System.Xml;
using DotNetty.Buffers;
using DotNetty.Codecs.Mqtt;
using DotNetty.Codecs.Mqtt.Packets;
using DotNetty.Handlers.Tls;
using DotNetty.Transport.Bootstrapping;
using DotNetty.Transport.Channels;
using DotNetty.Transport.Channels.Embedded;
using DotNetty.Transport.Channels.Sockets;
using InstaSharper.API.Push.MqttHelpers;
using InstaSharper.Classes.Android.DeviceInfo;

namespace InstaSharper.API.Push
{
    public class FbnsClient
    {
        private const string DEFAULT_HOST = "mqtt-mini.facebook.com";
        private const string DEFAULT_SERVICE = "https";
        private AndroidDevice _device;
        private FbnsConnectionData _connectionData;
        private MultithreadEventLoopGroup loopGroup = new MultithreadEventLoopGroup();

        public FbnsClient(AndroidDevice device, FbnsConnectionData connectionData = null)
        {
            _device = device;
            _connectionData = connectionData ?? LoadConnectionData();
            if (string.IsNullOrEmpty(_connectionData.UserAgent))
                _connectionData.UserAgent = FbnsUserAgent.BuildFbUserAgent(device);

            // Test data
//            _connectionData.ClientId = "6d5851bc-e2aa-4135-9";
//            _connectionData.UserId = 506457938464799;
//            _connectionData.Password = "BpUyH\\5G0XqpBp<QS<uh";
//            _connectionData.DeviceId = "6d5851bc-e2aa-4135-99c3-d55a67ac8110";
//            _connectionData.DeviceSecret = "6wST[83a9LnsJ?2M7cIo";

            FbnsTest().GetAwaiter();
        }

        public void SaveConnectionData()
        {
            // todo: implement save connection data to disk
        }

        public FbnsConnectionData LoadConnectionData()
        {
            // todo: implement load connection data from disk
            return new FbnsConnectionData();
        }

        public async Task FbnsTest()
        {
            var connectPacket = new FbnsConnectPacket
            {
                Payload = await PayloadProcessor.BuildPayload(_connectionData)
            };

            var tcpClient = new TcpClient(DEFAULT_HOST, 443);
            var secureStream = new SslStream(tcpClient.GetStream(), false);
            try
            {
                await secureStream.AuthenticateAsClientAsync(DEFAULT_HOST);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

            var embedded = new EmbeddedChannel();
            embedded.Pipeline.AddLast(new FbnsPacketEncoder());
            embedded.Pipeline.AddLast(new FbnsConnAckPacketDecoder(), new MqttDecoder(false, 1024));
            embedded.Pipeline.AddLast(new MqttHandler());
            embedded.WriteOutbound(connectPacket);
            var outBuffer = embedded.ReadOutbound<IByteBuffer>();
            while (embedded.OutboundMessages.Count > 0)
            {
                outBuffer.WriteBytes(embedded.ReadOutbound<IByteBuffer>());
            }
            var buf = new byte[outBuffer.ReadableBytes];
            outBuffer.ReadBytes(buf);
            await secureStream.WriteAsync(buf,0,buf.Length);
            await Task.Delay(2000);
            var inboundBuf = new byte[256];
            try
            {
                await secureStream.ReadAsync(inboundBuf, 0, inboundBuf.Length);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

//            var bootstrap = new Bootstrap();
//            bootstrap
//                .Group(loopGroup)
//                .Channel<TcpSocketChannel>()
//                .Option(ChannelOption.TcpNodelay, true)
//                .Option(ChannelOption.SoKeepalive, true)
//                .Handler(new ActionChannelInitializer<TcpSocketChannel>(channel =>
//                {
//                    var pipeline = channel.Pipeline;
//                    pipeline.AddLast(new TlsHandler(
//                        stream => new SslStream(stream, true, (sender, certificate, chain, errors) => true),
//                        new ClientTlsSettings(DEFAULT_HOST)));
//                    pipeline.AddLast(new FbnsPacketEncoder());
//                    pipeline.AddLast(new FbnsConnAckPacketDecoder(), new MqttDecoder(false, 1024));
//                    pipeline.AddLast(new MqttHandler());
//                }));
//
//            var bootstrapChannel = await bootstrap.ConnectAsync(new DnsEndPoint(DEFAULT_HOST, 443));
//
//
//            try
//            {
//                await bootstrapChannel.WriteAndFlushAsync(connectPacket);
//            }
//            catch (Exception e)
//            {
//                Console.WriteLine(e);
//                throw;
//            }
        }

        class MqttHandler : SimpleChannelInboundHandler<Packet>
        {
            protected override void ChannelRead0(IChannelHandlerContext ctx, Packet msg)
            {
                if (msg is FbnsConnAckPacket ack)
                {
                    Debug.WriteLine("Authentication data:");
                    Debug.WriteLine(ack.Authentication);
                }
                else
                {
                    Debug.WriteLine("Cannot get FbnsConnAckPacket");
                }

                ctx.Channel.CloseAsync();
            }
        }
    }
}
