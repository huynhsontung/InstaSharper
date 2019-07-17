using System.Collections.Generic;
using InstaSharper.Classes.ResponseWrappers.BaseResponse;
using Newtonsoft.Json;

namespace InstaSharper.Classes.ResponseWrappers.Collection
{
    public class InstaCollectionsResponse : BaseLoadableResponse
    {
        [JsonProperty("items")] public List<InstaCollectionItemResponse> Items { get; set; }
    }
}