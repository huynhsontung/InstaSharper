using InstaSharper.Classes.ResponseWrappers.BaseResponse;
using Newtonsoft.Json;

namespace InstaSharper.Classes.ResponseWrappers.Media
{
    public class InstaPermalinkResponse : BaseStatusResponse
    {
        [JsonProperty("permalink")] public string Permalink { get; set; }
    }
}