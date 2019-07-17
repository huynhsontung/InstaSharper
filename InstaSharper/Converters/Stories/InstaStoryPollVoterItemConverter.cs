/*
 * Developer: Ramtin Jokar [ Ramtinak@live.com ] [ My Telegram Account: https://t.me/ramtinak ]
 * 
 * Github source: https://github.com/ramtinak/InstagramApiSharp
 * Nuget package: https://www.nuget.org/packages/InstagramApiSharp
 * 
 * IRANIAN DEVELOPERS
 */

using System;
using InstaSharper.Classes.Models.Story;
using InstaSharper.Classes.ResponseWrappers.Story;
using InstaSharper.Helpers;

namespace InstaSharper.Converters.Stories
{
    internal class InstaStoryPollVoterItemConverter : IObjectConverter<InstaStoryVoterItem, InstaStoryVoterItemResponse>
    {
        public InstaStoryVoterItemResponse SourceObject { get; set; }

        public InstaStoryVoterItem Convert()
        {
            if (SourceObject == null) throw new ArgumentNullException($"Source object");

            var voterItem = new InstaStoryVoterItem
            {
                Vote = SourceObject.Vote ?? 0,
                Time = DateTimeHelper.FromUnixTimeSeconds(SourceObject.Ts),
                User = ConvertersFabric.Instance.GetUserShortFriendshipConverter(SourceObject.User).Convert()
            };

            return voterItem;
        }
    }
}
