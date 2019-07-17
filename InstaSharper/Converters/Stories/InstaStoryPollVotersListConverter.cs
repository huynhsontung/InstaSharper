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
    internal class InstaStoryPollVotersListConverter : IObjectConverter<InstaStoryPollVotersList, InstaStoryPollVotersListResponse>
    {
        public InstaStoryPollVotersListResponse SourceObject { get; set; }

        public InstaStoryPollVotersList Convert()
        {
            if (SourceObject == null) throw new ArgumentNullException($"Source object");

            var votersList = new InstaStoryPollVotersList
            {
                LatestPollVoteTime = DateTimeHelper.FromUnixTimeSeconds(SourceObject.LatestPollVoteTime ?? 0),
                MaxId = SourceObject.MaxId,
                MoreAvailable = SourceObject.MoreAvailable,
                PollId = SourceObject.PollId
            };

            if (SourceObject.Voters?.Count > 0)
                foreach (var voter in SourceObject.Voters)
                    votersList.Voters.Add(ConvertersFabric.Instance.GetStoryPollVoterItemConverter(voter).Convert());

            return votersList;
        }
    }
}
