using Newtonsoft.Json;

namespace InstaSharper.Classes.ResponseWrappers.Story
{
    public class InstaStoryMediaResponse
    {
        [JsonProperty("media")] public InstaStoryItemResponse Media { get; set; }
    }
}