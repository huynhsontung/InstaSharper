using System.Collections.Generic;
using InstaSharper.Classes.ResponseWrappers.Errors;
using InstaSharper.Classes.ResponseWrappers.User;
using Newtonsoft.Json;

namespace InstaSharper.Classes.ResponseWrappers.Media
{
    public class InstaMediaLikersResponse : BadStatusResponse
    {
        [JsonProperty("users")] public List<InstaUserShortResponse> Users { get; set; }

        [JsonProperty("user_count")] public int UsersCount { get; set; }
    }
}