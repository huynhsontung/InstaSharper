using Newtonsoft.Json;

namespace InstaSharper.Classes.ResponseWrappers.Media
{
    public class ImagesResponse
    {
        [JsonProperty("low_resolution")] public BaseMediaResponse LowResolution { get; set; }

        [JsonProperty("thumbnail")] public BaseMediaResponse Thumbnail { get; set; }

        [JsonProperty("standard_resolution")] public BaseMediaResponse StandartResolution { get; set; }
    }
}