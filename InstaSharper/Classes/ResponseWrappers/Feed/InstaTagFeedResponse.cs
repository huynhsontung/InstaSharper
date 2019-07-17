using System.Collections.Generic;
using InstaSharper.Classes.ResponseWrappers.Media;
using Newtonsoft.Json;

namespace InstaSharper.Classes.ResponseWrappers.Feed
{
    public class InstaTagFeedResponse : InstaMediaListResponse
    {
        [JsonProperty("ranked_items")]
        public List<InstaMediaItemResponse> RankedItems { get; set; } = new List<InstaMediaItemResponse>();
    }
}