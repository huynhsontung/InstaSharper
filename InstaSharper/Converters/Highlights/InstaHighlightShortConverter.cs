/*
 * Developer: Ramtin Jokar [ Ramtinak@live.com ] [ My Telegram Account: https://t.me/ramtinak ]
 * 
 * Github source: https://github.com/ramtinak/InstagramApiSharp
 * Nuget package: https://www.nuget.org/packages/InstagramApiSharp
 * 
 * IRANIAN DEVELOPERS
 */

using System;
using InstaSharper.Classes.Models.Highlight;
using InstaSharper.Classes.ResponseWrappers.Highlight;
using InstaSharper.Helpers;

namespace InstaSharper.Converters.Highlights
{
    internal class InstaHighlightShortConverter : IObjectConverter<InstaHighlightShort, InstaHighlightShortResponse>
    {
        public InstaHighlightShortResponse SourceObject { get; set; }

        public InstaHighlightShort Convert()
        {
            if (SourceObject == null) throw new ArgumentNullException($"Source object");

            var highlight = new InstaHighlightShort
            {
                Id = SourceObject.Id,
                LatestReelMedia = SourceObject.LatestReelMedia,
                MediaCount = SourceObject.MediaCount,
                ReelType = SourceObject.ReelType,
                Time = DateTimeHelper.FromUnixTimeSeconds(SourceObject.Timestamp ?? 0)
            };
            return highlight;
        }
    }
}
