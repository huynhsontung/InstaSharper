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
    internal class InstaBroadcastStartConverter : IObjectConverter<InstaBroadcastStart, InstaBroadcastStartResponse>
    {
        public InstaBroadcastStartResponse SourceObject { get; set; }

        public InstaBroadcastStart Convert()
        {
            if (SourceObject == null) throw new ArgumentNullException($"Source object");
            var broadcastStart = new InstaBroadcastStart
            {
                MediaId = SourceObject.MediaId
            };

            return broadcastStart;
        }
    }
}
