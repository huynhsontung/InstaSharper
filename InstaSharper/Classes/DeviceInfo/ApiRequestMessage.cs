using System;
using InstaSharper.API;
using InstaSharper.Helpers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace InstaSharper.Classes.DeviceInfo
{
    internal class ApiRequestChallengeMessage : ApiRequestMessage
    {
        [JsonProperty("_csrftoken")]
        public string CsrtToken { get; set; }
    }

    public class ApiRequestMessage
    {
        [JsonProperty("country_codes")] public JRaw CountryCodes = new JRaw("[{\"country_code\":\"1\",\"source\":[\"default\",\"sim\"]}]");
        [JsonProperty("phone_id")] public string PhoneId { get; set; }
        [JsonProperty("username")] public string Username { get; set; }
        [JsonProperty("adid")] public string AdId { get; set; }
        [JsonProperty("guid")] public Guid Guid { get; set; }
        [JsonProperty("_uuid")] public string Uuid => Guid.ToString();
        [JsonProperty("device_id")] public string DeviceId { get; set; }
        [JsonProperty("password")] public string Password { get; set; }
        [JsonProperty("login_attempt_count")] public string LoginAttemptCount { get; set; } = "0";

        internal string GetMessageString()
        {
            var json = JsonConvert.SerializeObject(this);
            return json;
        }

        internal string GetChallengeMessageString(string csrfToken)
        {
            var api = new ApiRequestChallengeMessage
            {
                CsrtToken = csrfToken,
                DeviceId = DeviceId,
                Guid = Guid,
                LoginAttemptCount = "1",
                Password = Password,
                PhoneId = PhoneId,
                Username = Username,
                AdId = AdId
            };
            var json = JsonConvert.SerializeObject(api);
            return json;
        }

        internal string GetMessageStringForChallengeVerificationCodeSend(int Choice = 1)
        {
            return JsonConvert.SerializeObject(new { choice = Choice.ToString(), _csrftoken = "ReplaceCSRF", Guid, DeviceId });
        }

        internal string GetChallengeVerificationCodeSend(string verify)
        {
            return JsonConvert.SerializeObject(new { security_code = verify, _csrftoken = "ReplaceCSRF", Guid, DeviceId });
        }

        internal string GenerateSignature(ApiVersion apiVersion, string signatureKey, out string deviceid)
        {
            if (string.IsNullOrEmpty(signatureKey))
                signatureKey = apiVersion.SignatureKey;
            var res = CryptoHelper.CalculateHash(signatureKey,
                JsonConvert.SerializeObject(this));
            deviceid = DeviceId;
            return res;
        }

        internal string GenerateChallengeSignature(ApiVersion apiVersion, string signatureKey, string csrfToken, out string deviceid)
        {
            if (string.IsNullOrEmpty(signatureKey))
                signatureKey = apiVersion.SignatureKey;
            var api = new ApiRequestChallengeMessage
            {
                CsrtToken = csrfToken,
                DeviceId = DeviceId,
                Guid = Guid,
                LoginAttemptCount = "1",
                Password = Password,
                PhoneId = PhoneId,
                Username = Username,
                AdId = AdId
            };
            var res = CryptoHelper.CalculateHash(signatureKey,
                JsonConvert.SerializeObject(api));
            deviceid = DeviceId;
            return res;
        }

        internal bool IsEmpty()
        {
            if (string.IsNullOrEmpty(PhoneId)) return true;
            if (string.IsNullOrEmpty(DeviceId)) return true;
            if (Guid.Empty == Guid) return true;
            return false;
        }

        internal static string GenerateDeviceId()
        {
            return GenerateDeviceIdFromGuid(Guid.NewGuid());
        }

        internal static string GenerateUploadId()
        {
            var timeSpan = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0);
            var uploadId = (long)timeSpan.TotalSeconds;
            return uploadId.ToString();
        }

        internal static string GenerateRandomUploadId()
        {
            return DateTime.UtcNow.ToUnixTimeMiliSeconds().ToString();
        }

        public static string GenerateDeviceIdFromGuid(Guid guid)
        {
            var hashedGuid = CryptoHelper.CalculateMd5(guid.ToString());
            return $"android-{hashedGuid.Substring(0, 16)}";
        }
    }
}