using Newtonsoft.Json;

namespace InstaSharper.Classes.ResponseWrappers.User
{
    public class FollowedByResponse
    {
        [JsonProperty("count")] public int Count { get; set; }
    }
}