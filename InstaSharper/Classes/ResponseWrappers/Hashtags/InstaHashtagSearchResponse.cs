using System.Collections.Generic;
using InstaSharper.Classes.ResponseWrappers.BaseResponse;
using Newtonsoft.Json;

namespace InstaSharper.Classes.ResponseWrappers.Hashtags
{
    public class InstaHashtagSearchResponse : BaseStatusResponse
    {
        [JsonIgnore] public List<InstaHashtagResponse> Tags { get; set; } = new List<InstaHashtagResponse>();

        [JsonProperty("has_more")] public bool? MoreAvailable { get; set; }

        [JsonProperty("rank_token")] public string RankToken { get; set; }
    }
}