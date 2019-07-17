using System.Collections.Generic;
using InstaSharper.Classes.Models.Media;
using InstaSharper.Classes.Models.Other;
using InstaSharper.Classes.Models.Story;
using InstaSharper.Classes.Models.User;

namespace InstaSharper.Classes.Models.Feed
{
    public class InstaFeed : IInstaBaseList
    {
        public int MediaItemsCount => Medias.Count;
        public int StoriesItemsCount => Stories.Count;

        public List<InstaMedia> Medias { get; set; } = new List<InstaMedia>();
        public List<InstaStory> Stories { get; set; } = new List<InstaStory>();
        public string NextMaxId { get; set; }

        public List<InstaSuggestionItem> SuggestedUserItems { get; set; } = new List<InstaSuggestionItem>();
    }
}