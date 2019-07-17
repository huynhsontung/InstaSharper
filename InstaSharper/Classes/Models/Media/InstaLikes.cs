using InstaSharper.Classes.Models.User;

namespace InstaSharper.Classes.Models.Media
{
    public class InstaLikes
    {
        public int Count { get; set; }
        public InstaUserList VisibleLikedUsers { get; set; }
    }
}