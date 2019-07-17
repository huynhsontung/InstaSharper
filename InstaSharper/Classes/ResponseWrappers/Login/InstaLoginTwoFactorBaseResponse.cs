using InstaSharper.Classes.Models.Challenge;
using Newtonsoft.Json;

namespace InstaSharper.Classes.ResponseWrappers.Login
{
    internal class InstaLoginTwoFactorBaseResponse
    {
        [JsonProperty("message")] public string Message { get; set; }

        [JsonProperty("error_type")] public string ErrorType { get; set; }

        [JsonProperty("challenge")] public InstaChallengeLoginInfo Challenge { get; set; }
    }
}