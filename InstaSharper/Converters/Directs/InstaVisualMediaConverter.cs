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
using InstaSharper.Classes.Models.Media;
using InstaSharper.Classes.ResponseWrappers.Direct;
using InstaSharper.Helpers;

namespace InstaSharper.Converters.Directs
{
    internal class InstaVisualMediaConverter : IObjectConverter<InstaVisualMedia, InstaVisualMediaResponse>
    {
        public InstaVisualMediaResponse SourceObject { get; set; }

        public InstaVisualMedia Convert()
        {
            if (SourceObject == null) throw new ArgumentNullException($"Source object");

            var visualMedia = new InstaVisualMedia
            {
                Height = SourceObject.Height ?? 0,
                InstaIdentifier = SourceObject.InstaIdentifier,
                MediaType = SourceObject.MediaType,
                MediaId = SourceObject.MediaId,
                TrackingToken = SourceObject.TrackingToken,
                Width = SourceObject.Width ?? 0
            };

            if (SourceObject.UrlExpireAtSecs != null)
                visualMedia.UrlExpireAt = SourceObject.UrlExpireAtSecs.Value.FromUnixTimeSeconds();

            if (SourceObject.Images?.Candidates != null)
                foreach (var image in SourceObject.Images.Candidates)
                    visualMedia.Images.Add(new InstaImage(image.Url, image.Width, image.Height));

            if (SourceObject.Videos?.Count > 0)
                foreach (var video in SourceObject.Videos)
                    visualMedia.Videos.Add(new InstaVideo(video.Url, video.Width, video.Height, video.Type));

            return visualMedia;
        }
    }
}
