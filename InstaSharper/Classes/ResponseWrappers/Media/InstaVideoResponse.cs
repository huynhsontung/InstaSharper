using Newtonsoft.Json;

namespace InstaSharper.Classes.ResponseWrappers.Media
{
    public class InstaVideoResponse : BaseMediaResponse
    {
        [JsonProperty("id")] public string Id { get; set; }
        [JsonProperty("type")] public int Type { get; set; }
    }
}