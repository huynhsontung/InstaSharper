/*
 * Developer: Ramtin Jokar [ Ramtinak@live.com ] [ My Telegram Account: https://t.me/ramtinak ]
 * 
 * Github source: https://github.com/ramtinak/InstagramApiSharp
 * Nuget package: https://www.nuget.org/packages/InstagramApiSharp
 * 
 * IRANIAN DEVELOPERS
 */

using System.Collections.Generic;
using InstaSharper.Classes.ResponseWrappers.Media;
using InstaSharper.Enums;
using Newtonsoft.Json;

namespace InstaSharper.Classes.ResponseWrappers.Direct
{
    public class InstaVisualMediaResponse
    {
        [JsonProperty("media_id")] public long MediaId { get; set; }

        [JsonProperty("id")] public string InstaIdentifier { get; set; }

        [JsonProperty("media_type")] public InstaMediaType MediaType { get; set; }

        [JsonProperty("image_versions2")] public InstaImageCandidatesResponse Images { get; set; }

        [JsonProperty("video_versions")] public List<InstaVideoResponse> Videos { get; set; }

        [JsonProperty("organic_tracking_token")] public string TrackingToken { get; set; }

        [JsonProperty("original_width")] public int? Width { get; set; }

        [JsonProperty("original_height")] public int? Height { get; set; }

        [JsonProperty("url_expire_at_secs")] public long? UrlExpireAtSecs { get; set; }
    }
}
