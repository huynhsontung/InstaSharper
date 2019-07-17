/*
 * Developer: Ramtin Jokar [ Ramtinak@live.com ] [ My Telegram Account: https://t.me/ramtinak ]
 * 
 * Github source: https://github.com/ramtinak/InstagramApiSharp
 * Nuget package: https://www.nuget.org/packages/InstagramApiSharp
 * 
 * IRANIAN DEVELOPERS
 */

using System;
using InstaSharper.Classes.Models.Broadcast;
using InstaSharper.Classes.ResponseWrappers.Broadcast;
using InstaSharper.Helpers;

namespace InstaSharper.Converters.Broadcast
{
    internal class InstaBroadcastLikeConverter : IObjectConverter<InstaBroadcastLike, InstaBroadcastLikeResponse>
    {
        public InstaBroadcastLikeResponse SourceObject { get; set; }

        public InstaBroadcastLike Convert()
        {
            if (SourceObject == null) throw new ArgumentNullException($"Source object");
            var broadcastLike = new InstaBroadcastLike
            {
                BurstLikes = SourceObject.BurstLikes,
                Likes = SourceObject.Likes,
                LikeTime = DateTimeHelper.FromUnixTimeSeconds(SourceObject.LikeTs ?? DateTime.Now.ToUnixTime())
            };
            return broadcastLike;
        }
    }
}
