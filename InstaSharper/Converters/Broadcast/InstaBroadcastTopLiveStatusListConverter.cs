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
    internal class InstaBroadcastTopLiveStatusListConverter : IObjectConverter<InstaBroadcastTopLiveStatusList, InstaBroadcastTopLiveStatusResponse>
    {
        public InstaBroadcastTopLiveStatusResponse SourceObject { get; set; }

        public InstaBroadcastTopLiveStatusList Convert()
        {
            if (SourceObject == null) throw new ArgumentNullException($"Source object");
            var broadcastStatusItems = new InstaBroadcastTopLiveStatusList();
            try
            {
                if (SourceObject.BroadcastStatusItems?.Count > 0)
                    foreach (var statusItem in SourceObject.BroadcastStatusItems)
                        broadcastStatusItems.Add(ConvertersFabric.Instance.GetBroadcastStatusItemConverter(statusItem).Convert());
            }
            catch { }
            return broadcastStatusItems;
        }
    }
}
