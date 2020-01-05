using System;
using InstaSharper.Classes.Models.Media;
using InstaSharper.Classes.ResponseWrappers.Media;

namespace InstaSharper.Converters.Media
{
    internal class InstaMediaImageConverter : IObjectConverter<InstaImage, BaseMediaResponse>
    {
        public BaseMediaResponse SourceObject { get; set; }

        public InstaImage Convert()
        {
            if (SourceObject == null) throw new ArgumentNullException($"Source object");
            var image = new InstaImage(SourceObject.Url, SourceObject.Width, SourceObject.Height);
            return image;
        }
    }
}