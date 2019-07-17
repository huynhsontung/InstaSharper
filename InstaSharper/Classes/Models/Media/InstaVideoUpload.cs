/*
 * Developer: Ramtin Jokar [ Ramtinak@live.com ] [ My Telegram Account: https://t.me/ramtinak ]
 * 
 * Github source: https://github.com/ramtinak/InstagramApiSharp
 * Nuget package: https://www.nuget.org/packages/InstagramApiSharp
 * 
 * IRANIAN DEVELOPERS
 */

using System.Collections.Generic;
using InstaSharper.Classes.Models.User;

namespace InstaSharper.Classes.Models.Media
{
    public class InstaVideoUpload
    {
        public InstaVideoUpload() { }
        public InstaVideoUpload(InstaVideo video, InstaImage videoThumbnail)
        {
            Video = video;
            VideoThumbnail = videoThumbnail;
        }
        public InstaVideo Video { get; set; }
        public InstaImage VideoThumbnail { get; set; }
        /// <summary>
        ///     User tags => Optional
        /// </summary>
        public List<InstaUserTagVideoUpload> UserTags { get; set; } = new List<InstaUserTagVideoUpload>();
    }
}
