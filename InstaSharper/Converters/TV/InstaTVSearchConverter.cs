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
using InstaSharper.Classes.Models.TV;
using InstaSharper.Classes.ResponseWrappers.TV;

namespace InstaSharper.Converters.TV
{
    internal class InstaTVSearchConverter : IObjectConverter<InstaTVSearch, InstaTVSearchResponse>
    {
        public InstaTVSearchResponse SourceObject { get; set; }

        public InstaTVSearch Convert()
        {
            if (SourceObject == null)
                throw new ArgumentNullException("SourceObject");

            var search = new InstaTVSearch
            {
                NumResults = SourceObject.NumResults ?? 0,
                Status = SourceObject.Status,
                RankToken = SourceObject.RankToken
            };

            if (SourceObject.Results != null && SourceObject.Results.Any())
            {
                foreach (var result in SourceObject.Results)
                {
                    try
                    {
                        search.Results.Add(ConvertersFabric.Instance.GetTVSearchResultConverter(result).Convert());
                    }
                    catch { }
                }
            }

            return search;
        }
    }
}
