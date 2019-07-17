using InstaSharper.Classes.Models.Media;

namespace InstaSharper.Classes.Models.User
{
    public class InstaUserTag
    {
        public InstaPosition Position { get; set; }

        public string TimeInVideo { get; set; }

        public InstaUserShort User { get; set; }
    }
}