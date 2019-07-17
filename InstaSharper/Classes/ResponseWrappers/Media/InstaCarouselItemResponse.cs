using System.Collections.Generic;
using InstaSharper.Classes.ResponseWrappers.User;
using InstaSharper.Enums;
using Newtonsoft.Json;

namespace InstaSharper.Classes.ResponseWrappers.Media
{
    public class InstaCarouselItemResponse
    {
        [JsonProperty("id")] public string InstaIdentifier { get; set; }

        [JsonProperty("media_type")] public InstaMediaType MediaType { get; set; }

        [JsonProperty("image_versions2")] public InstaImageCandidatesResponse Images { get; set; }

        [JsonProperty("video_versions")] public List<InstaVideoResponse> Videos { get; set; }

        [JsonProperty("original_width")] public string Width { get; set; }

        [JsonProperty("original_height")] public string Height { get; set; }

        [JsonProperty("pk")] public string Pk { get; set; }

        [JsonProperty("carousel_parent_id")] public string CarouselParentId { get; set; }

        [JsonProperty("usertags")] public InstaUserTagListResponse UserTagList { get; set; }
    }
}