/*
 * Developer: Ramtin Jokar [ Ramtinak@live.com ] [ My Telegram Account: https://t.me/ramtinak ]
 * 
 * Github source: https://github.com/ramtinak/InstagramApiSharp
 * Nuget package: https://www.nuget.org/packages/InstagramApiSharp
 * 
 * IRANIAN DEVELOPERS
 */

using InstaSharper.Classes.Models.Other;
using Newtonsoft.Json;

namespace InstaSharper.Classes.ResponseWrappers.Other
{
    public class InstaPresenceResponse : InstaDefault
    {
        [JsonProperty("disabled")]
        public bool? Disabled { get; set; }
        [JsonProperty("thread_presence_disabled")]
        public bool? ThreadPresenceDisabled { get; set; }
    }
}
