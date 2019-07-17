using Newtonsoft.Json;

namespace InstaSharper.Classes.ResponseWrappers.Media
{
    public class InstaWebLinkResponse
    {
        [JsonProperty("text")] public string Text { get; set; }

        [JsonProperty("link_context")] public InstaWebLinkContextResponse LinkContext { get; set; }
    }
}