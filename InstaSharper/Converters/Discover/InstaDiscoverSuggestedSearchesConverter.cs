/*
 * Developer: Ramtin Jokar [ Ramtinak@live.com ] [ My Telegram Account: https://t.me/ramtinak ]
 * 
 * Github source: https://github.com/ramtinak/InstagramApiSharp
 * Nuget package: https://www.nuget.org/packages/InstagramApiSharp
 * 
 * IRANIAN DEVELOPERS
 */

using System;
using System.Linq;
using InstaSharper.Classes.Models.Discover;
using InstaSharper.Classes.ResponseWrappers.Discover;

namespace InstaSharper.Converters.Discover
{
    internal class InstaDiscoverSuggestedSearchesConverter : 
        IObjectConverter<InstaDiscoverSuggestedSearches, InstaDiscoverSuggestedSearchesResponse>
    {
        public InstaDiscoverSuggestedSearchesResponse SourceObject { get; set; }

        public InstaDiscoverSuggestedSearches Convert()
        {
            if (SourceObject == null) throw new ArgumentNullException($"Source object");
            var suggested = new InstaDiscoverSuggestedSearches
            {
                RankToken = SourceObject.RankToken
            };
            if (SourceObject.Suggested != null && SourceObject.Suggested.Any())
            {
                foreach (var search in SourceObject.Suggested)
                {
                    try
                    {
                        suggested.Suggested.Add(ConvertersFabric.Instance.GetDiscoverSearchesConverter(search).Convert());
                    }
                    catch { }
                }
            }
            return suggested;
        }
    }
}
