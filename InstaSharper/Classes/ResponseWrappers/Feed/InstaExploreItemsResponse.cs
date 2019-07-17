using System.Collections.Generic;
using InstaSharper.Classes.ResponseWrappers.BaseResponse;
using InstaSharper.Classes.ResponseWrappers.Media;
using InstaSharper.Classes.ResponseWrappers.Story;
using Newtonsoft.Json;

namespace InstaSharper.Classes.ResponseWrappers.Feed
{
    public class InstaExploreItemsResponse : BaseLoadableResponse
    {
        [JsonIgnore] public InstaStoryTrayResponse StoryTray { get; set; } = new InstaStoryTrayResponse();

        [JsonIgnore] public List<InstaMediaItemResponse> Medias { get; set; } = new List<InstaMediaItemResponse>();

        [JsonIgnore] public InstaChannelResponse Channel { get; set; }
    }
}