/*
 * Developer: Ramtin Jokar [ Ramtinak@live.com ] [ My Telegram Account: https://t.me/ramtinak ]
 * 
 * Github source: https://github.com/ramtinak/InstagramApiSharp
 * Nuget package: https://www.nuget.org/packages/InstagramApiSharp
 * 
 * IRANIAN DEVELOPERS
 */

using System;
using InstaSharper.Classes.Models.Web;
using InstaSharper.Classes.ResponseWrappers.Web;
using InstaSharper.Helpers;

namespace InstaSharper.Converters.Web
{
    internal class InstaWebAccountInfoConverter : IObjectConverter<InstaWebAccountInfo, InstaWebSettingsPageResponse>
    {
        public InstaWebSettingsPageResponse SourceObject { get; set; }

        public InstaWebAccountInfo Convert()
        {
            if (SourceObject == null) throw new ArgumentNullException($"Source object");

            var info = new InstaWebAccountInfo();
            if (SourceObject.DateJoined?.Data?.Timestamp != null)
                info.JoinedDate = SourceObject.DateJoined?.Data?.Timestamp.Value.FromUnixTimeSeconds();
            else
                info.JoinedDate = DateTime.MinValue;

            if (SourceObject.SwitchedToBusiness?.Data?.Timestamp != null)
                info.SwitchedToBusinessDate = SourceObject.SwitchedToBusiness?.Data?.Timestamp.Value.FromUnixTimeSeconds();
            else
                info.SwitchedToBusinessDate = DateTime.MinValue;

            return info;
        }
    }
}
