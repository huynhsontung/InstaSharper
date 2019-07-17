/*
 * Developer: Ramtin Jokar [ Ramtinak@live.com ] [ My Telegram Account: https://t.me/ramtinak ]
 * 
 * Github source: https://github.com/ramtinak/InstagramApiSharp
 * Nuget package: https://www.nuget.org/packages/InstagramApiSharp
 * 
 * IRANIAN DEVELOPERS
 */

using System.Collections.Generic;
using InstaSharper.Classes.Models.Other;
using Newtonsoft.Json;

namespace InstaSharper.Classes.ResponseWrappers.User
{
    public class InstaBlockedUsersResponse : InstaDefault
    {
        [JsonProperty("blocked_list")]
        public List<InstaBlockedUserInfoResponse> BlockedList { get; set; }
        [JsonProperty("big_list")]
        public bool? BigList { get; set; }
        [JsonProperty("page_size")]
        public int? PageSize { get; set; }
        [JsonProperty("max_id")]
        public string MaxId { get; set; }
    }
}
