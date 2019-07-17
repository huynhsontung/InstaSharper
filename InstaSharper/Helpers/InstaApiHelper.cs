using InstaSharper.Classes;

namespace InstaSharper.Helpers
{
    internal class InstaApiHelper
    {
        public static string GetCodeFromId(long id)
        {
            var alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789-_".ToCharArray();
            var code = string.Empty;
            while (id > 0)
            {
                var remainder = id % 64;
                id = (id - remainder) / 64;
                code = alphabet[remainder] + code;
            }

            return code;
        }

        public static string GetCsrfToken(IHttpRequestProcessor requestProcessor)
        {
            var cookies =
                requestProcessor.HttpHandler.CookieContainer.GetCookies(requestProcessor.Client
                    .BaseAddress);
            return cookies["csrftoken"]?.Value ?? string.Empty;
        }
    }
}