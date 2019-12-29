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
using InstaSharper.Helpers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace InstaSharper.Classes.Models.Direct
{
    public class InstaLastSeen
    {
        public DateTime SeenTime => DateTimeHelper.UnixTimestampMicrosecondsToDateTime(TimestampPrivate);
        [JsonProperty("timestamp")] internal string TimestampPrivate { get; set; }
        [JsonProperty("item_id")] public string ItemId { get; set; }
    }

}
