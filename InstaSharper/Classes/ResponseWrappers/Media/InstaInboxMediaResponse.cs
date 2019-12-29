using System.Collections.Generic;
using InstaSharper.Enums;
using Newtonsoft.Json;

namespace InstaSharper.Classes.ResponseWrappers.Media
{
    public class InstaInboxMediaResponse
    {
        [JsonProperty("image_versions2")] public InstaImageCandidatesResponse ImageCandidates { get; set; }

        [JsonProperty("original_width")] public int OriginalWidth { get; set; }

        [JsonProperty("original_height")] public int OriginalHeight { get; set; }

        [JsonProperty("media_type")] public InstaMediaType MediaType { get; set; }

        [JsonProperty("video_versions")] public List<InstaVideoResponse> Videos { get; set; }
    }
}