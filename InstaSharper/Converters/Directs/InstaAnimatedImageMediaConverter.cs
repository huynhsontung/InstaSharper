/*
 * Developer: Ramtin Jokar [ Ramtinak@live.com ] [ My Telegram Account: https://t.me/ramtinak ]
 * 
 * Github source: https://github.com/ramtinak/InstagramApiSharp
 * Nuget package: https://www.nuget.org/packages/InstagramApiSharp
 * 
 * IRANIAN DEVELOPERS
 */

using System;
using InstaSharper.Classes.Models.Direct;
using InstaSharper.Classes.ResponseWrappers.Direct;

namespace InstaSharper.Converters.Directs
{
    internal class InstaAnimatedImageMediaConverter : IObjectConverter<InstaAnimatedImageMedia, InstaAnimatedImageMediaResponse>
    {
        public InstaAnimatedImageMediaResponse SourceObject { get; set; }

        public InstaAnimatedImageMedia Convert()
        {
            if (SourceObject == null) throw new ArgumentNullException($"Source object");

            var animatedMedia = new InstaAnimatedImageMedia
            {
                Height = int.Parse(SourceObject.Height ?? "0"),
                Mp4Url = SourceObject.Mp4,
                Mp4Size = int.Parse(SourceObject.Mp4Size ?? "0"),
                Size = int.Parse(SourceObject.Size ?? "0"),
                Url = SourceObject.Url,
                WebpUrl = SourceObject.Webp,
                WebpSize = int.Parse(SourceObject.WebpSize ?? "0"),
                Width = int.Parse(SourceObject.Width ?? "0")
            };

            return animatedMedia;
        }
    }
}
