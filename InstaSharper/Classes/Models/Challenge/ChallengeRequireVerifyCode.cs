/*
 * Developer: Ramtin Jokar [ Ramtinak@live.com ] [ My Telegram Account: https://t.me/ramtinak ]
 * 
 * Github source: https://github.com/ramtinak/InstagramApiSharp
 * Nuget package: https://www.nuget.org/packages/InstagramApiSharp
 * 
 * IRANIAN DEVELOPERS
 */

using InstaSharper.Classes.ResponseWrappers.User;
using Newtonsoft.Json;

namespace InstaSharper.Classes.Models.Challenge
{
    public class InstaChallengeRequireVerifyCode
    {
        [JsonIgnore]
        public bool IsLoggedIn { get { return LoggedInUser != null || Status.ToLower() == "ok"; } }
        [JsonProperty("logged_in_user")]
        public /*InstaUserInfoResponse*/InstaUserShortResponse LoggedInUser { get; set; }
        [JsonProperty("message")]
        public string Message { get; set; }
        [JsonProperty("status")]
        internal string Status { get; set; }
        [JsonProperty("action")]
        internal string Action { get; set; }
    }
}
