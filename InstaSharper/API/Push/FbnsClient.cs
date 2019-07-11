using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
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
using InstaSharper.API.Push.PacketHelpers;
using InstaSharper.Classes;
using InstaSharper.Classes.Android.DeviceInfo;
using InstaSharper.Helpers;

namespace InstaSharper.API.Push
{
    public sealed class FbnsClient
    {
//        public event EventHandler<string> MessageReceived;

        private readonly UserSessionData _user;
        private readonly IHttpRequestProcessor _httpRequestProcessor;
        private readonly AndroidDevice _device;
        private const string DEFAULT_HOST = "mqtt-mini.facebook.com";
        private const string DEFAULT_SERVICE = "https";
        private readonly MultithreadEventLoopGroup _loopGroup = new MultithreadEventLoopGroup();
        private IChannel _fbnsChannel;

        internal FbnsConnectionData ConnectionData { get; }

        public bool IsRunning => _fbnsChannel?.Open ?? false;

        internal FbnsClient(AndroidDevice device, UserSessionData sessionData, IHttpRequestProcessor requestProcessor, FbnsConnectionData connectionData = null)
        {
            _user = sessionData;
            _httpRequestProcessor = requestProcessor;
            _device = device;

            ConnectionData = connectionData ?? new FbnsConnectionData();

            // If token is older than 24 hours then discard it
            if ((DateTime.Now - ConnectionData.FbnsTokenLastUpdated).TotalHours > 24) ConnectionData.FbnsToken = "";

            // Build user agent for first time setup
            if (string.IsNullOrEmpty(ConnectionData.UserAgent))
                ConnectionData.UserAgent = FbnsUserAgent.BuildFbUserAgent(device);
        }

        internal async Task Start()
        {
            var connectPacket = new FbnsConnectPacket
            {
                Payload = await PayloadProcessor.BuildPayload(ConnectionData)
            };

            var bootstrap = new Bootstrap();
            bootstrap
                .Group(_loopGroup)
                .Channel<TcpSocketChannel>()
                .Option(ChannelOption.ConnectTimeout, TimeSpan.FromSeconds(5))
                .Option(ChannelOption.TcpNodelay, true)
                .Option(ChannelOption.SoKeepalive, true)
                .Handler(new ActionChannelInitializer<TcpSocketChannel>(channel =>
                {
                    var pipeline = channel.Pipeline;
                    pipeline.AddLast(new TlsHandler(
                        stream => new SslStream(stream, true, (sender, certificate, chain, errors) => true),
                        new ClientTlsSettings(DEFAULT_HOST)));
                    pipeline.AddLast(new FbnsPacketEncoder());
                    pipeline.AddLast(new FbnsPacketDecoder());
                    pipeline.AddLast(new PacketInboundHandler(this));
                }));

            _fbnsChannel = await bootstrap.ConnectAsync(new DnsEndPoint(DEFAULT_HOST, 443));

            try
            {
                await _fbnsChannel.WriteAndFlushAsync(connectPacket);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        internal async Task RegisterClient(string token)
        {
            if (string.IsNullOrEmpty(token)) throw new ArgumentNullException(nameof(token));
            if (ConnectionData.FbnsToken == token)
            {
                ConnectionData.FbnsToken = token;
                return;
            }
            
            var uri = UriCreator.GetRegisterPushUri();
            var fields = new Dictionary<string, string>()
            {
                {"device_type", "android_mqtt"},
                {"is_main_push_channel", "true"},
                {"phone_id", _device.PhoneId.ToString()},
                {"device_token", token},
                {"_csrftoken", _user.CsrfToken },
                {"guid", _device.Uuid.ToString() },
                {"_uuid", _device.Uuid.ToString() },
                {"users", _user.LoggedInUnder.Pk.ToString() }
            };

            var request = HttpHelper.GetDefaultRequest(HttpMethod.Post, uri, _device);
            request.Content = new FormUrlEncodedContent(fields);

            var response = await _httpRequestProcessor.SendAsync(request);

            ConnectionData.FbnsToken = token;
        }

        internal async Task Shutdown()
        {
            await _loopGroup.ShutdownGracefullyAsync();
        }
    }
}
