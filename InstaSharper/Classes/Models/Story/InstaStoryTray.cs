using System.Collections.Generic;
using InstaSharper.Classes.Models.Broadcast;

namespace InstaSharper.Classes.Models.Story
{
    public class InstaStoryTray
    {
        public long Id { get; set; }

        public InstaTopLive TopLive { get; set; } = new InstaTopLive();

        public bool IsPortrait { get; set; }

        public List<InstaStory> Tray { get; set; } = new List<InstaStory>();
    }
}