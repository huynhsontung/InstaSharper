using System;
using InstaSharper.Classes.Models.Feed;
using InstaSharper.Classes.ResponseWrappers.Feed;

namespace InstaSharper.Converters.Feeds
{
    internal class InstaFeedConverter : IObjectConverter<InstaFeed, InstaFeedResponse>
    {
        public InstaFeedResponse SourceObject { get; set; }

        public InstaFeed Convert()
        {
            if (SourceObject?.Items == null)
                throw new ArgumentNullException("InstaFeedResponse or its Items");
            var feed = new InstaFeed();
            foreach (var instaUserFeedItemResponse in SourceObject.Items)
            {
                if (instaUserFeedItemResponse?.Type != 0) continue;
                var feedItem = ConvertersFabric.Instance.GetSingleMediaConverter(instaUserFeedItemResponse).Convert();
                feed.Medias.Add(feedItem);
            }
            foreach (var suggestedItemResponse in SourceObject.SuggestedUsers)
            {
                try
                {
                    var suggestedItem = ConvertersFabric.Instance.GetSuggestionItemConverter(suggestedItemResponse).Convert();
                    feed.SuggestedUserItems.Add(suggestedItem);
                }
                catch { }
            }

            feed.NextMaxId = SourceObject.NextMaxId;
            return feed;
        }
    }
}