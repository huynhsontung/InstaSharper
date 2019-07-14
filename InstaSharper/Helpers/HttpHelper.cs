using System;
using System.Collections.Generic;
using System.Net.Http;
using InstaSharper.API;
using InstaSharper.Classes.Android.DeviceInfo;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace InstaSharper.Helpers
{
    internal static class HttpHelper
    {
        // todo: need to implement v2 api request
        public static HttpRequestMessage GetDefaultRequest(HttpMethod method, Uri uri, AndroidDevice deviceInfo)
        {
            var request = new HttpRequestMessage(method, uri);
            request.Headers.AcceptEncoding.ParseAdd(InstaApiConstants.ACCEPT_ENCODING);
            request.Headers.Connection.ParseAdd("Keep-Alive");
            request.Headers.UserAgent.ParseAdd(deviceInfo.UserAgent);
            request.Headers.Accept.ParseAdd("*/*");
            request.Headers.AcceptLanguage.ParseAdd(InstaApiConstants.ACCEPT_LANGUAGE);
            request.Headers.Add("X-IG-App-ID", InstaApiConstants.FACEBOOK_ANALYTICS_APPLICATION_ID);
            request.Headers.Add("X-IG-Capabilities", InstaApiConstants.IG_CAPABILITIES);
            request.Headers.Add("X-IG-Connection-Type", InstaApiConstants.IG_CONNECTION_TYPE);
            request.Headers.Add("X-FB-HTTP-Engine", InstaApiConstants.X_FB_HTTP_ENGINE);
            return request;
        }

        public static HttpRequestMessage GetSignedRequest(HttpMethod method,
            Uri uri,
            AndroidDevice deviceInfo,
            Dictionary<string, string> data)
        {
            var hash = CryptoHelper.CalculateHash(InstaApiConstants.IG_SIGNATURE_KEY,
                JsonConvert.SerializeObject(data));
            var payload = JsonConvert.SerializeObject(data);
            var signature = $"{hash}.{payload}";

            var fields = new Dictionary<string, string>
            {
                {"signed_body", signature},
                {"ig_sig_key_version", InstaApiConstants.IG_SIGNATURE_KEY_VERSION}
            };
            var request = GetDefaultRequest(HttpMethod.Post, uri, deviceInfo);
            request.Content = new FormUrlEncodedContent(fields);
            foreach (var field in fields)
            {
                request.Properties.Add(field.Key, field.Value);
            }
            return request;
        }

        public static HttpRequestMessage GetSignedRequest(HttpMethod method,
            Uri uri,
            AndroidDevice deviceInfo,
            JObject data)
        {
            var hash = CryptoHelper.CalculateHash(InstaApiConstants.IG_SIGNATURE_KEY,
                data.ToString(Formatting.None));
            var payload = data.ToString(Formatting.None);
            var signature = $"{hash}.{payload}";

            var fields = new Dictionary<string, string>
            {
                {"signed_body", signature},
                {"ig_sig_key_version", InstaApiConstants.IG_SIGNATURE_KEY_VERSION}
            };
            var request = GetDefaultRequest(HttpMethod.Post, uri, deviceInfo);
            request.Content = new FormUrlEncodedContent(fields);
            foreach (var field in fields)
            {
                request.Properties.Add(field.Key, field.Value);
            }
            return request;
        }
    }
}