/*
 * Developer: Ramtin Jokar [ Ramtinak@live.com ] [ My Telegram Account: https://t.me/ramtinak ]
 * 
 * Github source: https://github.com/ramtinak/InstagramApiSharp
 * Nuget package: https://www.nuget.org/packages/InstagramApiSharp
 * 
 * IRANIAN DEVELOPERS
 */

using System;
using InstaSharper.Classes.Models.Business;
using InstaSharper.Classes.ResponseWrappers.Business;
using InstaSharper.Enums;

namespace InstaSharper.Converters.Business
{
    internal class InstaMediaShortConverter : IObjectConverter<InstaMediaShort, InstaMediaShortResponse>
    {
        public InstaMediaShortResponse SourceObject { get; set; }

        public InstaMediaShort Convert()
        {
            var media = new InstaMediaShort
            {
               Id = SourceObject.Id,
               MediaIdentifier = SourceObject.MediaIdentifier
            };
            if (!string.IsNullOrEmpty(SourceObject.InstagramMediaType))
            {
                try
                {
                    media.MediaType = (InstaMediaType)Enum.Parse(typeof(InstaMediaType), SourceObject.InstagramMediaType, true);
                }
                catch { }
            }
            if (SourceObject.Image != null && SourceObject.Image.Uri != null)
                media.Image = SourceObject.Image.Uri;

            if (SourceObject.InlineInsightsNode != null)
            {
                try
                {
                    media.InsightsState = SourceObject.InlineInsightsNode.State;
                    media.MetricsImpressionsOrganicValue = 
                        SourceObject.InlineInsightsNode.Metrics.Impressions.Organic.Value ?? 0;
                }
                catch { }
            }
            
            return media;
        }
    }
}
