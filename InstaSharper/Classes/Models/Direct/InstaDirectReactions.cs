using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using InstaSharper.Helpers;

namespace InstaSharper.Classes.Models.Direct
{
    public class InstaDirectReactions
    {
        [JsonProperty("likes")] public List<InstaDirectLikeReaction> Likes { get; set; }
        [JsonProperty("likes_count")] public uint LikesCount { get; set; }
        public bool MeLiked { get; set; }
    }

    public class InstaDirectLikeReaction
    {
        [JsonProperty("sender_id")] public long SenderId { get; set; }
        [JsonProperty("client_context")] public string ClientContext { get; set; }
        [JsonProperty("timestamp")] public string TimestampUnix { get; set; }
        public DateTime Timestamp => DateTimeHelper.UnixTimestampMicrosecondsToDateTime(TimestampUnix);
    }
}