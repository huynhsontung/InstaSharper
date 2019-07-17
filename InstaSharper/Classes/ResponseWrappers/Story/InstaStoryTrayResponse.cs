using System.Collections.Generic;
using InstaSharper.Classes.ResponseWrappers.Broadcast;
using Newtonsoft.Json;

namespace InstaSharper.Classes.ResponseWrappers.Story
{
    public class InstaStoryTrayResponse
    {
        [JsonProperty("id")] public long Id { get; set; }

        [JsonProperty("top_live")] public InstaTopLiveResponse TopLive { get; set; } = new InstaTopLiveResponse();

        [JsonProperty("is_portrait")] public bool IsPortrait { get; set; }

        [JsonProperty("tray")] public List<InstaStoryResponse> Tray { get; set; } = new List<InstaStoryResponse>();
    }
}