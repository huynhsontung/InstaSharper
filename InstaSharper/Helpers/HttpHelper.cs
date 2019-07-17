using System;
using System.Collections.Generic;
using System.Net.Http;
using InstaSharper.API;
using InstaSharper.Classes;
using InstaSharper.Classes.DeviceInfo;
using InstaSharper.Enums;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace InstaSharper.Helpers
{
    internal static class HttpHelper
    {
        public static ApiVersion ApiVersion = ApiVersion.GetApiVersion(ApiVersionNumber.Version86);

        public static HttpRequestMessage GetDefaultRequest(HttpMethod method, Uri uri, AndroidDevice deviceInfo)
        {
            var request = new HttpRequestMessage(method, uri);
            request.Headers.Connection.ParseAdd("Keep-Alive");
            request.Headers.UserAgent.ParseAdd(deviceInfo.UserAgent);
            request.Headers.AcceptEncoding.ParseAdd(HttpRequestProcessor.ACCEPT_ENCODING);
            request.Headers.Accept.ParseAdd("*/*");
            request.Headers.AcceptLanguage.ParseAdd(InstaApiConstants.ACCEPT_LANGUAGE);
            request.Headers.Add("X-IG-Capabilities", "3brTBw==");
            request.Headers.Add("X-IG-Connection-Type", "WIFI");
            request.Headers.Add("X-IG-App-ID", InstaApiConstants.IG_APP_ID);
            request.Headers.Add("X-FB-HTTP-Engine", "Liger");
            request.Properties.Add(new KeyValuePair<string, object>("X-Google-AD-ID",
                deviceInfo.GoogleAdId.ToString()));
            return request;
        }

        public static HttpRequestMessage GetDefaultRequest(Uri uri, AndroidDevice deviceInfo, Dictionary<string, string> data)
        {
            var request = GetDefaultRequest(HttpMethod.Post, uri, deviceInfo);
            request.Content = new FormUrlEncodedContent(data);
            return request;
        }

        /// <summary>
        ///     This is only for https://instagram.com site
        /// </summary>
        public static HttpRequestMessage GetWebRequest(HttpMethod method, Uri uri, AndroidDevice deviceInfo)
        {
            var request = GetDefaultRequest(HttpMethod.Get, uri, deviceInfo);
            request.Headers.Remove("User-Agent");
            request.Headers.Add("User-Agent", InstaApiConstants.WEB_USER_AGENT);
            return request;
        }

        public static HttpRequestMessage GetSignedRequest(Uri uri,
            AndroidDevice deviceInfo,
            Dictionary<string, string> data)
        {
            var hash = CryptoHelper.CalculateHash(ApiVersion.SignatureKey,
                JsonConvert.SerializeObject(data));
            var payload = JsonConvert.SerializeObject(data);
            return GetSignedRequest(uri, deviceInfo, hash, payload);
        }

        public static HttpRequestMessage GetSignedRequest(Uri uri,
            AndroidDevice deviceInfo,
            JObject data)
        {
            var hash = CryptoHelper.CalculateHash(ApiVersion.SignatureKey,
                data.ToString(Formatting.None));
            var payload = data.ToString(Formatting.None);
            return GetSignedRequest(uri, deviceInfo, hash, payload);
        }

        private static HttpRequestMessage GetSignedRequest(Uri uri, AndroidDevice deviceInfo, string hash, string payload)
        {
            var signature = $"{hash}.{payload}";
            var fields = new Dictionary<string, string>
            {
                {InstaApiConstants.HEADER_IG_SIGNATURE, signature},
                {InstaApiConstants.HEADER_IG_SIGNATURE_KEY_VERSION, InstaApiConstants.IG_SIGNATURE_KEY_VERSION}
            };
            var request = GetDefaultRequest(HttpMethod.Post, uri, deviceInfo);
            request.Content = new FormUrlEncodedContent(fields);
            request.Properties.Add(InstaApiConstants.HEADER_IG_SIGNATURE, signature);
            request.Properties.Add(InstaApiConstants.HEADER_IG_SIGNATURE_KEY_VERSION,
                InstaApiConstants.IG_SIGNATURE_KEY_VERSION);
            return request;
        }

        public static string GetSignature(JObject data)
        {
            var hash = CryptoHelper.CalculateHash(ApiVersion.SignatureKey, data.ToString(Formatting.None));
            var payload = data.ToString(Formatting.None);
            var signature = $"{hash}.{payload}";
            return signature;
        }
    }
}