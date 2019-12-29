/*
 * Developer: Ramtin Jokar [ Ramtinak@live.com ] [ My Telegram Account: https://t.me/ramtinak ]
 * 
 * Github source: https://github.com/ramtinak/InstagramApiSharp
 * Nuget package: https://www.nuget.org/packages/InstagramApiSharp
 * 
 * IRANIAN DEVELOPERS
 */

using System;
using System.Collections.Generic;
using InstaSharper.Enums;

namespace InstaSharper.Classes.Models.Direct
{
    public class InstaVisualMediaContainer
    {
        public DateTime UrlExpireAt { get; set; }

        public InstaVisualMedia Media { get; set; }

        public int? SeenCount { get; set; }

        public DateTime ReplayExpiringAtUs { get; set; }

        public InstaViewMode ViewMode { get; set; }

        public List<long> SeenUserIds { get; set; } = new List<long>();

        public bool IsExpired => Media == null || Media.IsExpired;
    }
}
