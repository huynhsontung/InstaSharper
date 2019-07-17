using System.Collections.Generic;
using InstaSharper.Classes.ResponseWrappers.User;
using Newtonsoft.Json;

namespace InstaSharper.Classes.ResponseWrappers.Broadcast
{
    public class InstaTopLiveResponse
    {
        [JsonProperty("ranked_position")] public int RankedPosition { get; set; }

        [JsonProperty("broadcast_owners")]
        public List<InstaUserShortFriendshipFullResponse> BroadcastOwners { get; set; } = new List<InstaUserShortFriendshipFullResponse>();
    }
}