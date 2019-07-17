namespace InstaSharper.Classes.Models.User
{
    public class InstaInlineFollow
    {
        public bool IsOutgoingRequest { get; set; }
        public bool IsFollowing { get; set; }
        public InstaUserShort User { get; set; }
    }
}