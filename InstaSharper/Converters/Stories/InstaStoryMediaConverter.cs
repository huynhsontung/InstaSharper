using InstaSharper.Classes.Models.Story;
using InstaSharper.Classes.ResponseWrappers.Story;

namespace InstaSharper.Converters.Stories
{
    internal class InstaStoryMediaConverter : IObjectConverter<InstaStoryMedia, InstaStoryMediaResponse>
    {
        public InstaStoryMediaResponse SourceObject { get; set; }

        public InstaStoryMedia Convert()
        {
            var instaStoryMedia = new InstaStoryMedia
            {
                Media = ConvertersFabric.Instance.GetStoryItemConverter(SourceObject.Media).Convert()
            };

            return instaStoryMedia;
        }
    }
}