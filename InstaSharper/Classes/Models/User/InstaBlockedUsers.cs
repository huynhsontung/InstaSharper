using System.Collections.Generic;
using InstaSharper.Classes.Models.Other;

namespace InstaSharper.Classes.Models.User
{

    public class InstaBlockedUsers : InstaDefault
    {
        public List<InstaBlockedUserInfo> BlockedList { get; set; } = new List<InstaBlockedUserInfo>();
        
        public int? PageSize { get; set; }

        public string MaxId { get; set; }
    }
}
