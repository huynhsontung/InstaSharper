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

namespace InstaSharper.Classes.ResponseWrappers.Broadcast
{
    public class InstaBroadcastSendCommentResponse
    {
        [JsonProperty("content_type")]
        public string ContentType { get; set; }
        [JsonProperty("user")]
        public InstaUserShortFriendshipFullResponse User { get; set; }
        [JsonProperty("pk")]
        public long Pk { get; set; }
        [JsonProperty("text")]
        public string Text { get; set; }
        [JsonProperty("type")]
        public int Type { get; set; }
        [JsonProperty("created_at")]
        public long? CreatedAt { get; set; }
        [JsonProperty("created_at_utc")]
        public long? CreatedAtUtc { get; set; }
        [JsonProperty("media_id")]
        public long MediaId { get; set; }
        [JsonProperty("status")]
        public string Status { get; set; }
    }
}
