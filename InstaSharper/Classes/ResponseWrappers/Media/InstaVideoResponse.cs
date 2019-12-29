using Newtonsoft.Json;

namespace InstaSharper.Classes.ResponseWrappers.Media
{
    public class InstaVideoResponse
    {
        [JsonProperty("id")] public string Id { get; set; }

        [JsonProperty("url")] public string Url { get; set; }

        [JsonProperty("height")] public int Height { get; set; }

        [JsonProperty("type")] public int Type { get; set; }

        [JsonProperty("width")] public int Width { get; set; }
    }
}