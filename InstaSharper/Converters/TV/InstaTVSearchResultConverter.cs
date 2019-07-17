/*
 * Developer: Ramtin Jokar [ Ramtinak@live.com ] [ My Telegram Account: https://t.me/ramtinak ]
 * 
 * Github source: https://github.com/ramtinak/InstagramApiSharp
 * Nuget package: https://www.nuget.org/packages/InstagramApiSharp
 * 
 * IRANIAN DEVELOPERS
 */

using System;
using InstaSharper.Classes.Models.TV;
using InstaSharper.Classes.ResponseWrappers.TV;

namespace InstaSharper.Converters.TV
{
    internal class InstaTVSearchResultConverter : IObjectConverter<InstaTVSearchResult, InstaTVSearchResultResponse>
    {
        public InstaTVSearchResultResponse SourceObject { get; set; }

        public InstaTVSearchResult Convert()
        {
            if (SourceObject == null)
                throw new ArgumentNullException("SourceObject");

            var search = new InstaTVSearchResult
            {
                Type = SourceObject.Type
            };

            if (SourceObject.Channel != null)
            {
                try
                {
                    search.Channel = ConvertersFabric.Instance.GetTVChannelConverter(SourceObject.Channel).Convert();
                }
                catch { }
            }

            if (SourceObject.User != null)
            {
                try
                {
                    search.User = ConvertersFabric.Instance.GetUserShortFriendshipConverter(SourceObject.User).Convert();
                }
                catch { }
            }

            return search;
        }
    }
}
