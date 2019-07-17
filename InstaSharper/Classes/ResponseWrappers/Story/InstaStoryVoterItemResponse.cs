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

namespace InstaSharper.Classes.ResponseWrappers.Story
{
    public class InstaStoryVoterItemResponse
    {
        [JsonProperty("user")]
        public InstaUserShortFriendshipResponse User { get; set; }
        [JsonProperty("vote")]
        public double? Vote { get; set; }
        [JsonProperty("ts")]
        public long Ts { get; set; }
    }
}
