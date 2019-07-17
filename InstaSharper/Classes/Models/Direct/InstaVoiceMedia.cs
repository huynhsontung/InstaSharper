/*
 * Developer: Ramtin Jokar [ Ramtinak@live.com ] [ My Telegram Account: https://t.me/ramtinak ]
 * 
 * Github source: https://github.com/ramtinak/InstagramApiSharp
 * Nuget package: https://www.nuget.org/packages/InstagramApiSharp
 * 
 * IRANIAN DEVELOPERS
 */

using System.Collections.Generic;
using InstaSharper.Enums;

namespace InstaSharper.Classes.Models.Direct
{
    public class InstaVoiceMedia
    {
        public InstaVoice Media { get; set; }

        public List<long> SeenUserIds { get; set; } = new List<long>();

        public InstaViewMode ViewMode { get; set; }

        public int? SeenCount { get; set; }

        public string ReplayExpiringAtUs { get; set; }
    }
}
