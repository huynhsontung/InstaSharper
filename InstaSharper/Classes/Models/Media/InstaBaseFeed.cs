using InstaSharper.Classes.Models.Other;

namespace InstaSharper.Classes.Models.Media
{
    public class InstaBaseFeed : IInstaBaseList
    {
        public InstaMediaList Medias { get; set; } = new InstaMediaList();
        public string NextMaxId { get; set; }
    }
}