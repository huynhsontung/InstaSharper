using InstaSharper.Classes.Models.Media;
using InstaSharper.Classes.Models.Story;

namespace InstaSharper.Classes.Models.Location
{
    public class InstaLocationFeed : InstaBaseFeed
    {
        public InstaMediaList RankedMedias { get; set; } = new InstaMediaList();
        public InstaStory Story { get; set; }
        public InstaLocation Location { get; set; }

        public long MediaCount { get; set; }
    }
}