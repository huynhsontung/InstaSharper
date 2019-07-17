using System.Collections.Generic;
using InstaSharper.Classes.Models.Other;
using InstaSharper.Classes.Models.User;

namespace InstaSharper.Classes.Models.Feed
{
    public class InstaActivityFeed : IInstaBaseList
    {
        public bool IsOwnActivity { get; set; } = false;
        public List<InstaRecentActivityFeed> Items { get; set; } = new List<InstaRecentActivityFeed>();
        public string NextMaxId { get; set; }
    }
}