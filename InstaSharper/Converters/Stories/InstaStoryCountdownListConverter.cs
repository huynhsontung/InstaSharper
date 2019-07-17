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

namespace InstaSharper.Converters.Stories
{
    internal class InstaStoryCountdownListConverter : IObjectConverter<InstaStoryCountdownList, InstaStoryCountdownListResponse>
    {
        public InstaStoryCountdownListResponse SourceObject { get; set; }

        public InstaStoryCountdownList Convert()
        {
            if (SourceObject == null) throw new ArgumentNullException($"Source object");

            var storyCountdownList = new InstaStoryCountdownList
            {
                MoreAvailable = SourceObject.MoreAvailable ?? false,
                MaxId = SourceObject.MaxId
            };

            if (SourceObject.Items?.Count > 0)
                foreach(var countdown in SourceObject.Items)
                    storyCountdownList.Items.Add(ConvertersFabric.Instance.GetStoryCountdownStickerItemConverter(countdown).Convert());

            return storyCountdownList;
        }
    }
}
