using System.Collections.Generic;
using InstaSharper.Classes.Models.User;

namespace InstaSharper.Classes.Models.Broadcast
{
    public class InstaTopLive
    {
        public int RankedPosition { get; set; }

        public List<InstaUserShortFriendshipFull> BroadcastOwners { get; set; } = new List<InstaUserShortFriendshipFull>();
    }
}