using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net.Sockets;
using System.Text;
using DotNetty.Buffers;
using DotNetty.Codecs.Mqtt.Packets;
using DotNetty.Transport.Channels;
using InstaSharper.API.Push.PacketHelpers;
using Ionic.Zlib;
using Newtonsoft.Json;

namespace InstaSharper.API.Push
{
    internal class PacketInboundHandler : SimpleChannelInboundHandler<Packet>
    {
        public enum TopicIds
        {
            Message = 76,   // "/fbns_msg"
            RegReq = 79,    // "/fbns_reg_req"
            RegResp = 80    // "/fbns_reg_resp"
        }

        private readonly FbnsClient _client;

        private int _publishId;

//        private static bool _waiting = false;
        private const int TIMEOUT = 5;
        private const int NUM_OF_RETRIES = 5;

        public PacketInboundHandler(FbnsClient client)
        {
            _client = client;
        }

        public override void ChannelInactive(IChannelHandlerContext context)
        {
            base.ChannelInactive(context);
        }

        protected override void ChannelRead0(IChannelHandlerContext ctx, Packet msg)
        {
            switch (msg.PacketType)
            {
                case PacketType.CONNACK:
                    _client.ConnectionData.UpdateAuth(((FbnsConnAckPacket) msg).Authentication);

                    RegisterMqttClient(ctx);
                    break;
                case PacketType.PUBLISH:
                    var publishPacket = (PublishPacket) msg;
                    switch (Enum.Parse(typeof(TopicIds), publishPacket.TopicName))
                    {
                        case TopicIds.Message:
                            // todo: handle general message
                            break;
                        case TopicIds.RegResp:
                            OnRegisterResponse(publishPacket.Payload);
                            break;
                        default:
                            Debug.WriteLine($"Unknown topic received: {publishPacket.TopicName}", "Warning");
                            break;
                    }
                    break;
                // Todo: Handle other packet types
            }
        }

        /// Referencing from https://github.com/mgp25/Instagram-API/blob/master/src/Push.php
        /// <summary>
        ///     After receiving the token, proceed to register over Instagram API
        /// </summary>
        private void OnRegisterResponse(IByteBuffer payload)
        {
            var totalLength = payload.ReadableBytes;

            var decompressedStream = new MemoryStream(256);
            using (var compressedStream = new MemoryStream(totalLength))
            {
                payload.GetBytes(0, compressedStream, totalLength);
                payload.Release();
                compressedStream.Position = 0;
                using (var zlibStream = new ZlibStream(compressedStream, CompressionMode.Decompress, true))
                {
                    zlibStream.CopyTo(decompressedStream);
                }
            }

            var data = new byte[decompressedStream.Length];
            decompressedStream.Position = 0;
            decompressedStream.Read(data, 0, data.Length);
            decompressedStream.Dispose();
            var json = Encoding.UTF8.GetString(data);

            var response = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);

            if (!string.IsNullOrEmpty(response["error"]))
            {
                Debug.WriteLine("FBNS error: " + response["error"], "Error");
                return;
            }

            var token = response["token"];

            var task = _client.RegisterClient(token);
        }


        /// <summary>
        ///     Register this client on the MQTT side stating what application this client is using.
        ///     The server will then return a token for registering over Instagram API side.
        /// </summary>
        /// <param name="ctx"></param>
        private void RegisterMqttClient(IChannelHandlerContext ctx)
        {
            var message = new Dictionary<string, string>
            {
                {"pkg_name", InstaApiConstants.PACKAGE_NAME},
                {"appid", InstaApiConstants.FACEBOOK_ANALYTICS_APPLICATION_ID}
            };

            var json = JsonConvert.SerializeObject(message);
            var jsonBytes = Encoding.UTF8.GetBytes(json);
            byte[] compressed;
            using (var compressedStream = new MemoryStream(jsonBytes.Length))
            {
                using (var zlibStream =
                    new ZlibStream(compressedStream, CompressionMode.Compress, CompressionLevel.Level9, true))
                {
                    zlibStream.Write(jsonBytes, 0, jsonBytes.Length);
                }
                compressed = new byte[compressedStream.Length];
                compressedStream.Position = 0;
                compressedStream.Read(compressed, 0, compressed.Length);
            }

            var publishPacket = new PublishPacket(QualityOfService.AtLeastOnce, false, false)
            {
                Payload = Unpooled.CopiedBuffer(compressed),
                PacketId = ++_publishId,
                TopicName = ((byte)TopicIds.RegReq).ToString()
            };

            ctx.WriteAndFlushAsync(publishPacket);
        }
    }
}
