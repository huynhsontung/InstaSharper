using System;
using InstaSharper.Classes.Models.Direct;
using InstaSharper.Classes.Models.Media;
using InstaSharper.Classes.ResponseWrappers.Media;

namespace InstaSharper.Converters.Directs
{
    internal class InstaInboxMediaConverter : IObjectConverter<InstaInboxMedia, InstaInboxMediaResponse>
    {
        public InstaInboxMediaResponse SourceObject { get; set; }

        public InstaInboxMedia Convert()
        {
            if (SourceObject == null) throw new ArgumentNullException($"Source object");
            var inboxMedia = new InstaInboxMedia
            {
                MediaType = SourceObject.MediaType,
                OriginalHeight = SourceObject.OriginalHeight,
                OriginalWidth = SourceObject.OriginalWidth
            };
            if (SourceObject?.ImageCandidates?.Candidates == null) return inboxMedia;
            foreach (var image in SourceObject.ImageCandidates.Candidates)
                inboxMedia.Images.Add(new InstaImage(image.Url, image.Width, image.Height));
            
            if (SourceObject.Videos?.Count > 0)
                foreach (var video in SourceObject.Videos)
                    inboxMedia.Videos.Add(new InstaVideo(video.Url, video.Width, video.Height,
                        video.Type));

            return inboxMedia;
        }
    }
}