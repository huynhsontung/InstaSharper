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
using InstaSharper.Classes.Models.User;
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

    public class InstaVoice
    {
        public string Id { get; set; }

        public int MediaType { get; set; }

        public string ProductType { get; set; }

        public InstaAudio Audio { get; set; }

        public string OrganicTrackingToken { get; set; }

        public InstaFriendshipStatus FriendshipStatus { get; set; }
    }

    public class InstaAudio
    {
        public string AudioSource { get; set; }

        private double _duration;
        public double Duration { get => _duration; set { _duration = value; DurationTs = System.TimeSpan.FromMilliseconds(value); } }

        public TimeSpan DurationTs { get; set; }

        public float[] WaveformData { get; set; }

        public int WaveformSamplingFrequencyHz { get; set; }
    }
}
