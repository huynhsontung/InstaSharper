using System.Collections.Generic;
using InstaSharper.Classes.ResponseWrappers.BaseResponse;
using InstaSharper.Classes.ResponseWrappers.Story;
using Newtonsoft.Json;

namespace InstaSharper.Classes.ResponseWrappers.Media
{
    public class InstaMediaListResponse : BaseLoadableResponse
    {
        [JsonProperty("items")]
        public List<InstaMediaItemResponse> Medias { get; set; } = new List<InstaMediaItemResponse>();

        public List<InstaStoryResponse> Stories { get; set; } = new List<InstaStoryResponse>();
    }
}