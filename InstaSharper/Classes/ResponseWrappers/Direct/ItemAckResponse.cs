using System;
using System.Collections.Generic;
using System.Text;
using InstaSharper.Classes.Models.Other;
using InstaSharper.Helpers;
using Newtonsoft.Json;

namespace InstaSharper.Classes.ResponseWrappers.Direct
{
    public class ItemAckResponse : InstaDefault
    {
        [JsonProperty("action")] public string Action { get; set; }
        [JsonProperty("payload")] public ItemAckPayloadResponse Payload { get; set; }
    }

    public class ItemAckPayloadResponse
    {
        [JsonProperty("client_context")] public string ClientContext { get; set; }
        [JsonProperty("item_id")] public string ItemId { get; set; }
        [JsonProperty("thread_id")] public string ThreadId { get; set; }
        [JsonProperty("timestamp")] public string TimestampUnix { get; set; }
        [JsonIgnore] public DateTime Timestamp => DateTimeHelper.UnixTimestampMicrosecondsToDateTime(TimestampUnix);

    }
}
