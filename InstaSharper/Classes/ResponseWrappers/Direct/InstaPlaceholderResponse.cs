using Newtonsoft.Json;

namespace InstaSharper.Classes.ResponseWrappers.Direct
{
    public class InstaPlaceholderResponse
    {
        [JsonProperty("is_linked")] public bool IsLinked { get; set; }
        [JsonProperty("message")] public string Message { get; set; }
    }
}
