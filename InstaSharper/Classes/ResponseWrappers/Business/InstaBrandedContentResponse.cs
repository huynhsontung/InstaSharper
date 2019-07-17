/*
 * Developer: Ramtin Jokar [ Ramtinak@live.com ] [ My Telegram Account: https://t.me/ramtinak ]
 * 
 * Github source: https://github.com/ramtinak/InstagramApiSharp
 * Nuget package: https://www.nuget.org/packages/InstagramApiSharp
 * 
 * IRANIAN DEVELOPERS
 */

using System.Collections.Generic;
using InstaSharper.Classes.ResponseWrappers.User;
using Newtonsoft.Json;

namespace InstaSharper.Classes.ResponseWrappers.Business
{
    public class InstaBrandedContentResponse
    {
        [JsonProperty("require_approval")]
        public bool RequireApproval { get; set; }
        [JsonProperty("whitelisted_users")]
        public List<InstaUserShortResponse> WhitelistedUsers { get; set; }
        [JsonProperty("status")]
        public string Status { get; set; }
    }
}
