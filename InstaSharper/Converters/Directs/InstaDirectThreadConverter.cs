using System.Collections.Generic;
using System.Linq;
using InstaSharper.Classes.Models.Direct;
using InstaSharper.Classes.ResponseWrappers.Direct;
using InstaSharper.Helpers;
using Newtonsoft.Json;

namespace InstaSharper.Converters.Directs
{
    internal class InstaDirectThreadConverter : IObjectConverter<InstaDirectInboxThread, InstaDirectInboxThreadResponse>
    {
        public InstaDirectInboxThreadResponse SourceObject { get; set; }

        public InstaDirectInboxThread Convert()
        {
            var thread = new InstaDirectInboxThread
            {
                Canonical = SourceObject.Canonical,
                HasNewer = SourceObject.HasNewer,
                HasOlder = SourceObject.HasOlder,
                IsSpam = SourceObject.IsSpam,
                Muted = SourceObject.Muted,
                Named = SourceObject.Named,
                Pending = SourceObject.Pending,
                ViewerId = SourceObject.ViewerId,
                LastActivity = DateTimeHelper.UnixTimestampMilisecondsToDateTime(SourceObject.LastActivity),
                LastNonSenderItemAt = DateTimeHelper.UnixTimestampMilisecondsToDateTime(SourceObject.LastNonSenderItemAt),
                ThreadId = SourceObject.ThreadId,
                OldestCursor = SourceObject.OldestCursor,
                IsGroup = SourceObject.IsGroup,
                IsPin = SourceObject.IsPin,
                ValuedRequest = SourceObject.ValuedRequest,
                PendingScore = SourceObject.PendingScore ?? 0,
                VCMuted = SourceObject.VCMuted,
                ReshareReceiveCount = SourceObject.ReshareReceiveCount,
                ReshareSendCount = SourceObject.ReshareSendCount,
                ExpiringMediaReceiveCount = SourceObject.ExpiringMediaReceiveCount,
                ExpiringMediaSendCount = SourceObject.ExpiringMediaSendCount,
                NewestCursor = SourceObject.NewestCursor,
                ThreadType = SourceObject.ThreadType,
                Title = SourceObject.Title,
            
                MentionsMuted = SourceObject.MentionsMuted ?? false
            };

            if (SourceObject.Inviter != null)
            {
                var userConverter = ConvertersFabric.Instance.GetUserShortConverter(SourceObject.Inviter);
                thread.Inviter = userConverter.Convert();
            }

            if (SourceObject.Items != null && SourceObject.Items.Count > 0)
            {
                thread.Items = new List<InstaDirectInboxItem>();
                foreach (var item in SourceObject.Items)
                {
                    var converter = ConvertersFabric.Instance.GetDirectThreadItemConverter(item);
                    var threadItem = converter.Convert();
                    threadItem.FromMe = threadItem.UserId == thread.ViewerId;
                    AddCustomText(ref threadItem);
                    thread.Items.Add(threadItem);
                }
            }

            if (SourceObject.LastPermanentItem != null)
            {
                var converter = ConvertersFabric.Instance.GetDirectThreadItemConverter(SourceObject.LastPermanentItem);
                var lastPermanentItem = converter.Convert();
                AddCustomText(ref lastPermanentItem);
                thread.LastPermanentItem = lastPermanentItem;
            }
            if (SourceObject.Users != null && SourceObject.Users.Count > 0)
            {
                foreach (var user in SourceObject.Users)
                {
                    var converter = ConvertersFabric.Instance.GetUserShortFriendshipConverter(user);
                    thread.Users.Add(converter.Convert());
                }
            }

            if (SourceObject.LeftUsers != null && SourceObject.LeftUsers.Count > 0)
            {
                foreach (var user in SourceObject.LeftUsers)
                {
                    var converter = ConvertersFabric.Instance.GetUserShortFriendshipConverter(user);
                    thread.LeftUsers.Add(converter.Convert());
                }
            }

            if (SourceObject.LastSeenAt != null && SourceObject.LastSeenAt != null)
            {
                try
                {
                    var lastSeenJson = System.Convert.ToString(SourceObject.LastSeenAt);
                    var obj = JsonConvert.DeserializeObject<InstaLastSeenAtResponse>(lastSeenJson);
                    thread.LastSeenAt = new List<InstaLastSeen>();
                    foreach (var extraItem in obj.Extras)
                    {
                        var convertedLastSeen = JsonConvert.DeserializeObject<InstaLastSeenItemResponse>(extraItem.Value.ToString(Formatting.None));
                        var lastSeen = new InstaLastSeen
                        {
                            PK = long.Parse(extraItem.Key),
                            ItemId = convertedLastSeen.ItemId,
                        };
                        if (convertedLastSeen.TimestampPrivate != null)
                            lastSeen.SeenTime = DateTimeHelper.UnixTimestampMilisecondsToDateTime(convertedLastSeen.TimestampPrivate);
                        thread.LastSeenAt.Add(lastSeen);
                    }
                }
                catch { }
            }
            try
            {
                var viewer = thread.LastSeenAt.Single(x => thread.ViewerId == x.PK);
                thread.HasUnreadMessage = thread.LastNonSenderItemAt > viewer.SeenTime;
            }
            catch 
            {
                thread.HasUnreadMessage = false;
            }
            

            return thread;
        }

        private static void AddCustomText(ref InstaDirectInboxItem item)
        {
            switch (item.ItemType)
            {
                case InstaDirectThreadItemType.ReelShare:
                    switch (item.ReelShareMedia.Type)
                    {
                        case "reaction":
                            item.Text = item.FromMe
                                ? $"You reacted to their story {item.ReelShareMedia.Text}"
                                : $"Reacted to your story {item.ReelShareMedia.Text}";
                            break;
                        case "reply":
                            item.Text = item.FromMe ? "You replied to their story" : "Replied to your story";
                            break;
                        case "mention":
                            item.Text = item.FromMe ? "You mentioned them in your story" : "Mentioned you in their story";
                            break;
                    }
                    break;

                case InstaDirectThreadItemType.RavenMedia:
                    item.Text = item.FromMe ? "You sent them a photo" : "Sent you a photo";
                    break;

                case InstaDirectThreadItemType.ActionLog:
                    item.Text = item.ActionLog.Description;
                    break;

            }
        }
    }
}