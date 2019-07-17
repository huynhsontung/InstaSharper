/*
 * Developer: Ramtin Jokar [ Ramtinak@live.com ] [ My Telegram Account: https://t.me/ramtinak ]
 * 
 * Github source: https://github.com/ramtinak/InstagramApiSharp
 * Nuget package: https://www.nuget.org/packages/InstagramApiSharp
 * 
 * IRANIAN DEVELOPERS
 */

using InstaSharper.Classes.Models.User;
using InstaSharper.Classes.ResponseWrappers.User;

namespace InstaSharper.Converters.Users
{
    internal class InstaAdsInfoConverter : IObjectConverter<InstaAdsInfo, InstaAdsInfoResponse>
    {
        public InstaAdsInfoResponse SourceObject { get; set; }

        public InstaAdsInfo Convert()
        {
            return new InstaAdsInfo()
            {
                AdsUrl = SourceObject.AdsUrl,
                HasAds = SourceObject.HasAds ?? false
            };
        }
    }
}
