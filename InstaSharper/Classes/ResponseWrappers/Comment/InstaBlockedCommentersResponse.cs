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
using InstaSharper.Classes.ResponseWrappers.User;
using Newtonsoft.Json;

namespace InstaSharper.Classes.ResponseWrappers.Comment
{
    public class InstaBlockedCommentersResponse : InstaDefault
    {
        [JsonProperty("count")]
        public int Count { get; set; }
        [JsonProperty("blocked_commenters")]
        public List<InstaUserShortResponse> BlockedCommenters { get; set; }
    }

}
