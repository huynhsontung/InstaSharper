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
    internal class InstaStorySliderVoterInfoItemConverter : IObjectConverter<InstaStorySliderVoterInfoItem, InstaStorySliderVoterInfoItemResponse>
    {
        public InstaStorySliderVoterInfoItemResponse SourceObject { get; set; }

        public InstaStorySliderVoterInfoItem Convert()
        {
            if (SourceObject == null) throw new ArgumentNullException($"Source object");

            var voterInfoItem = new InstaStorySliderVoterInfoItem
            {
                LatestSliderVoteTime = DateTimeHelper.FromUnixTimeSeconds(SourceObject.LatestSliderVoteTime ?? DateTime.Now.ToUnixTime()),
                MaxId = SourceObject.MaxId,
                MoreAvailable = SourceObject.MoreAvailable,
                SliderId = SourceObject.SliderId
            };

            if (SourceObject.Voters?.Count > 0)
                foreach (var voter in SourceObject.Voters)
                    voterInfoItem.Voters.Add(ConvertersFabric.Instance.GetStoryPollVoterItemConverter(voter).Convert());

            return voterInfoItem;
        }
    }
}
