using InstaSharper.Classes.Models.Other;
using Newtonsoft.Json;

namespace InstaSharper.Classes.ResponseWrappers.User
{
    public class InstaSuggestionUserDetailContainerResponse : InstaDefault
    {
        [JsonProperty("items")]
        public InstaSuggestionItemListResponse Items { get; set; } = new InstaSuggestionItemListResponse();
    }
}
