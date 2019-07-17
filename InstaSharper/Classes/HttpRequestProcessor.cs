using System;
using System.ComponentModel.Design;
using System.IO;
using System.IO.Compression;
using System.Net.Http;
using System.Threading.Tasks;
using InstaSharper.Classes.DeviceInfo;
using InstaSharper.Logger;

namespace InstaSharper.Classes
{
    internal class HttpRequestProcessor : IHttpRequestProcessor
    {
        private readonly IRequestDelay _delay;
        private readonly IInstaLogger _logger;

        public HttpRequestProcessor(IRequestDelay delay, HttpClient httpClient, HttpClientHandler httpHandler,
            ApiRequestMessage requestMessage, IInstaLogger logger)
        {
            _delay = delay;
            Client = httpClient;
            Client.Timeout = TimeSpan.FromSeconds(20);
            HttpHandler = httpHandler;
            RequestMessage = requestMessage;
            _logger = logger;
        }

        public HttpClientHandler HttpHandler { get; }
        public ApiRequestMessage RequestMessage { get; }
        public HttpClient Client { get; }

        public async Task<HttpResponseMessage> SendAsync(HttpRequestMessage requestMessage)
        {
            LogHttpRequest(requestMessage);
            if (_delay.Exist)
                await Task.Delay(_delay.Value);
            var response = await Client.SendAsync(requestMessage);
            response.Content = await DecompressHttpContent(response.Content);
            LogHttpResponse(response);
            return response;
        }

        public async Task<HttpResponseMessage> GetAsync(Uri requestUri)
        {
            _logger?.LogRequest(requestUri);
            if (_delay.Exist)
                await Task.Delay(_delay.Value);
            var response = await Client.GetAsync(requestUri);
            response.Content = await DecompressHttpContent(response.Content);
            LogHttpResponse(response);
            return response;
        }

        public async Task<HttpResponseMessage> SendAsync(HttpRequestMessage requestMessage,
            HttpCompletionOption completionOption)
        {
            LogHttpRequest(requestMessage);
            if (_delay.Exist)
                await Task.Delay(_delay.Value);
            var response = await Client.SendAsync(requestMessage, completionOption);
            response.Content = await DecompressHttpContent(response.Content);
            LogHttpResponse(response);
            return response;
        }

        public async Task<string> SendAndGetJsonAsync(HttpRequestMessage requestMessage,
            HttpCompletionOption completionOption)
        {
            LogHttpRequest(requestMessage);
            if (_delay.Exist)
                await Task.Delay(_delay.Value);
            var response = await Client.SendAsync(requestMessage, completionOption);
            response.Content = await DecompressHttpContent(response.Content);
            LogHttpResponse(response);
            return await response.Content.ReadAsStringAsync();
        }

        public async Task<string> GetJsonAsync(Uri requestUri)
        {
            _logger?.LogRequest(requestUri);
            if (_delay.Exist)
                await Task.Delay(_delay.Value);
            var response = await Client.GetAsync(requestUri);
            response.Content = await DecompressHttpContent(response.Content);
            LogHttpResponse(response);
            return await response.Content.ReadAsStringAsync();
        }

        private void LogHttpRequest(HttpRequestMessage request)
        {
            _logger?.LogRequest(request);
        }

        private void LogHttpResponse(HttpResponseMessage request)
        {
            _logger?.LogResponse(request);
        }

        private static async Task<HttpContent> DecompressHttpContent(HttpContent content)
        {
            var encoding = content.Headers.ContentEncoding;
            var isGzip = encoding.Contains("gzip");
            var isDeflate = encoding.Contains("deflate");
            if (!isGzip && !isDeflate && encoding.Count != 0)
            {
                throw new ArgumentException("DecompressHttpContent: Compression type not supported.");
            }

            if (encoding.Count == 0)
            {
                return content;
            }

            var decompressed = new MemoryStream(16384);
            var data = await content.ReadAsStreamAsync();
            if (isDeflate)
            {
                using (var deflateStream = new DeflateStream(data, CompressionMode.Decompress))
                {
                    await deflateStream.CopyToAsync(decompressed);
                }
            }
            else if(isGzip)
            {
                using (var gzipStream = new GZipStream(data, CompressionMode.Decompress))
                {
                    await gzipStream.CopyToAsync(decompressed);
                }
            }

            decompressed.Position = 0;
            var newContent = new StreamContent(decompressed);
            newContent.Headers.ContentType = content.Headers.ContentType;
            newContent.Headers.ContentLanguage.Add(content.Headers.ContentLanguage.ToString());
            newContent.Headers.ContentLength = decompressed.Length;
            return newContent;
        }
    }
}