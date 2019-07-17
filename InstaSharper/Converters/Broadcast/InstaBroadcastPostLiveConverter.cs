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

namespace InstaSharper.Converters.Broadcast
{
    internal class InstaBroadcastPostLiveConverter : IObjectConverter<InstaBroadcastPostLive, InstaBroadcastPostLiveResponse>
    {
        public InstaBroadcastPostLiveResponse SourceObject { get; set; }

        public InstaBroadcastPostLive Convert()
        {
            if (SourceObject == null) throw new ArgumentNullException($"Source object");
            var postLive = new InstaBroadcastPostLive
            {
                PeakViewerCount = SourceObject.PeakViewerCount,
                Pk = SourceObject.Pk
            };

            if (SourceObject.User != null)
                postLive.User = ConvertersFabric.Instance
                    .GetUserShortFriendshipFullConverter(SourceObject.User).Convert();
            try
            {
                if (SourceObject.Broadcasts?.Count > 0)
                    foreach (var broadcastInfo in SourceObject.Broadcasts)
                        postLive.Broadcasts.Add(ConvertersFabric.Instance.GetBroadcastInfoConverter(broadcastInfo).Convert());
            }
            catch { }
            return postLive;
        }
    }
}
