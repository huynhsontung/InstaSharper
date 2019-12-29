using Newtonsoft.Json;

namespace InstaSharper.Classes.ResponseWrappers.Media
{
    public class ImageResponse
    {
        [JsonProperty("url")] public string Url { get; set; }

        [JsonProperty("width")] public int Width { get; set; }

        [JsonProperty("height")] public int Height { get; set; }
    }
}