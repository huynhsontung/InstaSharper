using System;
using System.Net.Http;
using System.Threading.Tasks;
using InstaSharper.Classes.DeviceInfo;

namespace InstaSharper.Classes
{
    public interface IHttpRequestProcessor
    {
        HttpClientHandler HttpHandler { get; }
        ApiRequestMessage RequestMessage { get; }
        HttpClient Client { get; }
        void SetHttpClientHandler(HttpClientHandler handler);
        Task<HttpResponseMessage> SendAsync(HttpRequestMessage requestMessage);
        Task<HttpResponseMessage> GetAsync(Uri requestUri);
        Task<HttpResponseMessage> SendAsync(HttpRequestMessage requestMessage, HttpCompletionOption completionOption);
        Task<string> SendAndGetJsonAsync(HttpRequestMessage requestMessage, HttpCompletionOption completionOption);
        Task<string> GetJsonAsync(Uri requestUri);
        IRequestDelay Delay { get; set; }
    }
}