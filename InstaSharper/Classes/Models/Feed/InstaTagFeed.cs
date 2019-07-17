using System.Collections.Generic;
using InstaSharper.Classes.Models.Media;

namespace InstaSharper.Classes.Models.Feed
{
    public class InstaTagFeed : InstaFeed
    {
        public List<InstaMedia> RankedMedias { get; set; } = new List<InstaMedia>();
    }
}