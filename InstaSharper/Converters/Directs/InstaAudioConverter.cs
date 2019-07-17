/*
 * Developer: Ramtin Jokar [ Ramtinak@live.com ] [ My Telegram Account: https://t.me/ramtinak ]
 * 
 * Github source: https://github.com/ramtinak/InstagramApiSharp
 * Nuget package: https://www.nuget.org/packages/InstagramApiSharp
 * 
 * IRANIAN DEVELOPERS
 */

using System;
using InstaSharper.Classes.Models.Direct;
using InstaSharper.Classes.ResponseWrappers.Direct;

namespace InstaSharper.Converters.Directs
{
    internal class InstaAudioConverter : IObjectConverter<InstaAudio, InstaAudioResponse>
    {
        public InstaAudioResponse SourceObject { get; set; }

        public InstaAudio Convert()
        {
            if (SourceObject == null) throw new ArgumentNullException($"Source object");

            var audio = new InstaAudio
            {
                AudioSource = SourceObject.AudioSource,
                Duration = SourceObject.Duration,
                WaveformData = SourceObject.WaveformData,
                WaveformSamplingFrequencyHz = SourceObject.WaveformSamplingFrequencyHz
            };

            return audio;
        }
    }
}
