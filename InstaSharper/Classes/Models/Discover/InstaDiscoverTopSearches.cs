using System.Collections.Generic;

namespace InstaSharper.Classes.Models.Discover
{
    public class InstaDiscoverTopSearches
    {
        public string RankToken { get; set; }

        public List<InstaDiscoverSearches> TopResults { get; set; } = new List<InstaDiscoverSearches>();
    }
}
