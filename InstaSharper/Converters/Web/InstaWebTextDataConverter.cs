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
    internal class InstaWebTextDataConverter : IObjectConverter<InstaWebTextData, InstaWebSettingsPageResponse>
    {
        public InstaWebSettingsPageResponse SourceObject { get; set; }

        public InstaWebTextData Convert()
        {
            if (SourceObject == null) throw new ArgumentNullException($"Source object");

            var list = new InstaWebTextData();
            if (SourceObject.Data.Data?.Count > 0)
            {
                foreach (var item in SourceObject.Data.Data)
                {
                    if (item.Text.IsNotEmpty())
                        list.Items.Add(item.Text);
                }
                list.MaxId = SourceObject.Data.Cursor;
            }
            return list;
        }
    }
}
