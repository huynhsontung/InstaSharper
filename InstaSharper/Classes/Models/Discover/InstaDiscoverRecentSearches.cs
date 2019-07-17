/*
 * Developer: Ramtin Jokar [ Ramtinak@live.com ] [ My Telegram Account: https://t.me/ramtinak ]
 * 
 * Github source: https://github.com/ramtinak/InstagramApiSharp
 * Nuget package: https://www.nuget.org/packages/InstagramApiSharp
 * 
 * IRANIAN DEVELOPERS
 */

using System.Collections.Generic;

namespace InstaSharper.Classes.Models.Discover
{
    public class InstaDiscoverRecentSearches
    {
        public List<InstaDiscoverSearches> Recent { get; set; } = new List<InstaDiscoverSearches>();
    }
}
