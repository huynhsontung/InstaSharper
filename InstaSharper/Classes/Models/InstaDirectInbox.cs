using System;
using System.Collections.Generic;

namespace InstaSharper.Classes.Models
{
    public class InstaDirectInbox
    {
        public bool HasOlder { get; set; }

        public DateTime UnseenCountTs { get; set; } // Timestamp

        public long UnseenCount { get; set; }

        public string OldestCursor { get; set; }

        public List<InstaDirectInboxThread> Threads { get; set; }
    }
}