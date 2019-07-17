using System;
using InstaSharper.Classes.Models.Media;
using InstaSharper.Classes.ResponseWrappers.Media;

namespace InstaSharper.Converters.Media
{
    internal class InstaMediaImageConverter : IObjectConverter<InstaImage, ImageResponse>
    {
        public ImageResponse SourceObject { get; set; }

        public InstaImage Convert()
        {
            if (SourceObject == null) throw new ArgumentNullException($"Source object");
            var image = new InstaImage(SourceObject.Url, int.Parse(SourceObject.Width), int.Parse(SourceObject.Height));
            return image;
        }
    }
}