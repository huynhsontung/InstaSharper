/*
 * Developer: Ramtin Jokar [ Ramtinak@live.com ] [ My Telegram Account: https://t.me/ramtinak ]
 * 
 * Github source: https://github.com/ramtinak/InstagramApiSharp
 * Nuget package: https://www.nuget.org/packages/InstagramApiSharp
 * 
 * IRANIAN DEVELOPERS
 */

using System.Collections.Generic;
using Newtonsoft.Json;

namespace InstaSharper.Classes.ResponseWrappers.Story
{
    public class InstaStoryPollStickerItemResponse
    {
        [JsonProperty("id")]
        public string Id { get; set; }
        [JsonProperty("poll_id")]
        public long PollId { get; set; }
        [JsonProperty("question")]
        public string Question { get; set; }
        [JsonProperty("tallies")]
        public List<InstaStoryTalliesItemResponse> Tallies { get; set; }
        [JsonProperty("viewer_can_vote")]
        public bool ViewerCanVote { get; set; }
        [JsonProperty("is_shared_result")]
        public bool IsSharedResult { get; set; }
        [JsonProperty("finished")]
        public bool Finished { get; set; }
        [JsonProperty("viewer_vote")]
        public long? ViewerVote { get; set; }
    }
}
