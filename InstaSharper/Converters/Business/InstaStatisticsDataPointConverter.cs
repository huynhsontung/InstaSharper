/*
 * Developer: Ramtin Jokar [ Ramtinak@live.com ] [ My Telegram Account: https://t.me/ramtinak ]
 * 
 * Github source: https://github.com/ramtinak/InstagramApiSharp
 * Nuget package: https://www.nuget.org/packages/InstagramApiSharp
 * 
 * IRANIAN DEVELOPERS
 */

using InstaSharper.Classes.Models.Business;
using InstaSharper.Classes.ResponseWrappers.Business;

namespace InstaSharper.Converters.Business
{
    internal class InstaStatisticsDataPointConverter : IObjectConverter<InstaStatisticsDataPointItem, InstaStatisticsDataPointItemResponse>
    {
        public InstaStatisticsDataPointItemResponse SourceObject { get; set; }

        public InstaStatisticsDataPointItem Convert()
        {
            var dataPoint = new InstaStatisticsDataPointItem
            {
                Label = SourceObject.Label,
                Value = SourceObject.Value ?? 0
            };
            return dataPoint;
        }
    }
}
