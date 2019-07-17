/*
 * Developer: Ramtin Jokar [ Ramtinak@live.com ] [ My Telegram Account: https://t.me/ramtinak ]
 * 
 * Github source: https://github.com/ramtinak/InstagramApiSharp
 * Nuget package: https://www.nuget.org/packages/InstagramApiSharp
 * 
 * IRANIAN DEVELOPERS
 */

using System.Collections.Generic;
using InstaSharper.Classes.ResponseWrappers.BaseResponse;
using InstaSharper.Classes.ResponseWrappers.Media;
using InstaSharper.Classes.ResponseWrappers.TV;
using Newtonsoft.Json;

namespace InstaSharper.Classes.ResponseWrappers.Feed
{
    public class InstaTopicalExploreFeedResponse : BaseLoadableResponse
    {
        [JsonProperty("clusters")] public List<InstaTopicalExploreClusterResponse> Clusters { get; set; } = new List<InstaTopicalExploreClusterResponse>();

        [JsonIgnore] public List<InstaMediaItemResponse> Medias { get; set; } = new List<InstaMediaItemResponse>();

        [JsonIgnore] public InstaChannelResponse Channel { get; set; }

        [JsonIgnore] public List<InstaTVChannelResponse> TVChannels { get; set; } = new List<InstaTVChannelResponse>();

        [JsonProperty("max_id")] public string MaxId { get; set; }

        [JsonProperty("has_shopping_channel_content")] public bool? HasShoppingChannelContent { get; set; }
    }
}
