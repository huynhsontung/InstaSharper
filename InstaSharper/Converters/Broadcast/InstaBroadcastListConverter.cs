/*
 * Developer: Ramtin Jokar [ Ramtinak@live.com ] [ My Telegram Account: https://t.me/ramtinak ]
 * 
 * Github source: https://github.com/ramtinak/InstagramApiSharp
 * Nuget package: https://www.nuget.org/packages/InstagramApiSharp
 * 
 * IRANIAN DEVELOPERS
 */

using System.Collections.Generic;
using InstaSharper.Classes.Models.Broadcast;
using InstaSharper.Classes.ResponseWrappers.Broadcast;

namespace InstaSharper.Converters.Broadcast
{
    internal class InstaBroadcastListConverter : IObjectConverter<InstaBroadcastList, List<InstaBroadcastResponse>>
    {
        public List<InstaBroadcastResponse> SourceObject { get; set; }

        public InstaBroadcastList Convert()
        {
            //if (SourceObject == null) throw new ArgumentNullException($"Source object");
            var broadcastList = new InstaBroadcastList();
            if (SourceObject?.Count > 0)
                foreach (var broadcast in SourceObject)
                    broadcastList.Add(ConvertersFabric.Instance.GetBroadcastConverter(broadcast).Convert());

            return broadcastList;
        }
    }
}
