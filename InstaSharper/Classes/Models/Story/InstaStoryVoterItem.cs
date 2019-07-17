/*
 * Developer: Ramtin Jokar [ Ramtinak@live.com ] [ My Telegram Account: https://t.me/ramtinak ]
 * 
 * Github source: https://github.com/ramtinak/InstagramApiSharp
 * Nuget package: https://www.nuget.org/packages/InstagramApiSharp
 * 
 * IRANIAN DEVELOPERS
 */

using System;
using InstaSharper.Classes.Models.User;

namespace InstaSharper.Classes.Models.Story
{
    public class InstaStoryVoterItem
    {
        public InstaUserShortFriendship User { get; set; }

        public double Vote { get; set; }

        public DateTime Time { get; set; }
    }
}
