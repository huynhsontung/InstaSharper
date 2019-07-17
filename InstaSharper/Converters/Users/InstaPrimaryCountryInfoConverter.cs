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
    internal class InstaPrimaryCountryInfoConverter : IObjectConverter<InstaPrimaryCountryInfo, InstaPrimaryCountryInfoResponse>
    {
        public InstaPrimaryCountryInfoResponse SourceObject { get; set; }

        public InstaPrimaryCountryInfo Convert()
        {
            return new InstaPrimaryCountryInfo()
            {
                CountryName = SourceObject.CountryName,
                HasCountry = SourceObject.HasCountry ?? false,
                IsVisible = SourceObject.IsVisible ?? false
            };
        }
    }
}
