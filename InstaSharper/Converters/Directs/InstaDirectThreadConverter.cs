using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using InstaSharper.Classes.Models.Direct;
using InstaSharper.Classes.ResponseWrappers.Direct;
using InstaSharper.Enums;
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
                LastSeenAt = SourceObject.LastSeenAt,
                LastActivity = DateTimeHelper.UnixTimestampMicrosecondsToDateTime(SourceObject.LastActivity),
                LastNonSenderItemAt = DateTimeHelper.UnixTimestampMicrosecondsToDateTime(SourceObject.LastNonSenderItemAt),
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
                    if (threadItem.Reactions != null)
                        threadItem.Reactions.MeLiked = threadItem.Reactions.Likes.Any(x => x.SenderId == thread.ViewerId);
                    AddCustomText(ref threadItem);
                    thread.Items.Add(threadItem);
                }
            }

            if (SourceObject.LastPermanentItem != null)
            {
                var converter = ConvertersFabric.Instance.GetDirectThreadItemConverter(SourceObject.LastPermanentItem);
                var lastPermanentItem = converter.Convert();
                lastPermanentItem.FromMe = lastPermanentItem.UserId == thread.ViewerId;
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

            if (thread.LastSeenAt != null && thread.LastSeenAt.TryGetValue(thread.ViewerId, out var viewerLastSeen))
            {
                thread.HasUnreadMessage = thread.LastNonSenderItemAt > viewerLastSeen.SeenTime &&
                                          thread.LastActivity == thread.LastNonSenderItemAt;
            }
            else
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
                    var mediaType = item.RavenMedia?.MediaType ?? item.VisualMedia.Media.MediaType;
                    if (mediaType == InstaMediaType.Image)
                        item.Text = item.FromMe ? "You sent them a photo" : "Sent you a photo";
                    else
                        item.Text = item.FromMe ? "You sent them a video" : "Sent you a video";
                    break;

                case InstaDirectThreadItemType.ActionLog:
                    item.Text = item.ActionLog.Description;
                    break;

                case InstaDirectThreadItemType.Link:
                    // item.Text = item.FromMe ? "You sent them a link" : "Sent you a link";
                    break;

                case InstaDirectThreadItemType.Media:
                    if (item.Media.MediaType == InstaMediaType.Image)
                        item.Text = item.FromMe ? "You sent them a photo" : "Sent you a photo";
                    else
                        item.Text = item.FromMe ? "You sent them a video" : "Sent you a video";
                    break;

                case InstaDirectThreadItemType.MediaShare:
                    item.Text = item.FromMe ? "You shared a post" : "Shared a post";
                    break;

                case InstaDirectThreadItemType.StoryShare:
                    item.Text = item.FromMe ? "You shared a story" : "Shared a story";
                    break;

                case InstaDirectThreadItemType.VoiceMedia:
                    item.Text = item.FromMe ? "You sent a voice clip" : "Sent you a voice clip";
                    break;

                case InstaDirectThreadItemType.AnimatedMedia:
                    item.Text = item.FromMe ? "You sent a GIF" : "Sent you a GIF";
                    break;

                case InstaDirectThreadItemType.Profile:
                    item.Text = item.FromMe ? "You shared a profile with them" : "Shared a profile with you";
                    break;

                case InstaDirectThreadItemType.Placeholder:
                    break;

                case InstaDirectThreadItemType.Location:
                    item.Text = item.FromMe ? "You shared a location with them" : "Shared a location with you";
                    break;
            }
        }
    }
}