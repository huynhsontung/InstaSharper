﻿using System;
using System.Collections.Generic;
using System.Linq;
using InstaSharper.Classes.Models.Direct;
using InstaSharper.Classes.Models.Location;
using InstaSharper.Classes.Models.Media;
using InstaSharper.Classes.Models.Story;
using InstaSharper.Classes.ResponseWrappers.Direct;
using InstaSharper.Enums;
using InstaSharper.Helpers;

namespace InstaSharper.Converters.Directs
{
    public class InstaDirectThreadItemConverter : IObjectConverter<InstaDirectInboxItem, InstaDirectInboxItemResponse>
    {
        public InstaDirectInboxItemResponse SourceObject { get; set; }

        public InstaDirectInboxItem Convert()
        {
            var threadItem = new InstaDirectInboxItem
            {
                ClientContext = SourceObject.ClientContext,
                ItemId = SourceObject.ItemId,
                Reactions = SourceObject.Reactions,
                TimeStamp = DateTimeHelper.UnixTimestampMicrosecondsToDateTime(SourceObject.TimeStamp),
                UserId = SourceObject.UserId
            };


            var truncatedItemType = SourceObject.ItemType?.Trim().Replace("_", "");
            if (Enum.TryParse(truncatedItemType, true, out InstaDirectThreadItemType type))
                threadItem.ItemType = type;
            else
                threadItem.ItemType = InstaDirectThreadItemType.Unknown;

            switch (threadItem.ItemType)
            {
                case InstaDirectThreadItemType.Link when SourceObject.Link != null:
                    threadItem.Text = SourceObject.Link.Text;
                    try
                    {
                        threadItem.LinkMedia = new InstaWebLink
                        {
                            Text = SourceObject.Link.Text
                        };
                        if (SourceObject.Link.LinkContext != null)
                        {
                            threadItem.LinkMedia.LinkContext = new InstaWebLinkContext();

                            if (!string.IsNullOrEmpty(SourceObject.Link.LinkContext.LinkImageUrl))
                                threadItem.LinkMedia.LinkContext.LinkImageUrl = SourceObject.Link.LinkContext.LinkImageUrl;

                            if (!string.IsNullOrEmpty(SourceObject.Link.LinkContext.LinkSummary))
                                threadItem.LinkMedia.LinkContext.LinkSummary = SourceObject.Link.LinkContext.LinkSummary;

                            if (!string.IsNullOrEmpty(SourceObject.Link.LinkContext.LinkTitle))
                                threadItem.LinkMedia.LinkContext.LinkTitle = SourceObject.Link.LinkContext.LinkTitle;

                            if (!string.IsNullOrEmpty(SourceObject.Link.LinkContext.LinkUrl))
                                threadItem.LinkMedia.LinkContext.LinkUrl = SourceObject.Link.LinkContext.LinkUrl;
                        }
                    }
                    catch { }

                    break;
                case InstaDirectThreadItemType.Like:
                    threadItem.Text = SourceObject.Like;
                    break;
                case InstaDirectThreadItemType.Media when SourceObject.Media != null:
                {
                    var converter = ConvertersFabric.Instance.GetInboxMediaConverter(SourceObject.Media);
                    threadItem.Media = converter.Convert();
                    break;
                }
                case InstaDirectThreadItemType.MediaShare when SourceObject.MediaShare != null:
                {
                    var converter = ConvertersFabric.Instance.GetSingleMediaConverter(SourceObject.MediaShare);
                    threadItem.MediaShare = converter.Convert();
                    break;
                }
                case InstaDirectThreadItemType.StoryShare when SourceObject.StoryShare != null:
                {
                    threadItem.StoryShare = new InstaStoryShare
                    {
                        IsReelPersisted = SourceObject.StoryShare.IsReelPersisted,
                        ReelType = SourceObject.StoryShare.ReelType,
                        Text = SourceObject.StoryShare.Text,
                        IsLinked = SourceObject.StoryShare.IsLinked,
                        Message = SourceObject.StoryShare.Message,
                        Title = SourceObject.StoryShare.Title
                    };
                    if (SourceObject.StoryShare.Media != null)
                    {
                        var converter = ConvertersFabric.Instance.GetSingleMediaConverter(SourceObject.StoryShare.Media);
                        threadItem.StoryShare.Media = converter.Convert();
                    }

                    break;
                }
                case InstaDirectThreadItemType.Text:
                    threadItem.Text = SourceObject.Text;
                    break;
                // VisualMedia is updated RavenMedia for v61 and newer
                case InstaDirectThreadItemType.RavenMedia when SourceObject.RavenMedia != null:
                {
                    var converter = ConvertersFabric.Instance.GetVisualMediaConverter(SourceObject.RavenMedia);
                    threadItem.RavenMedia = converter.Convert();
                    threadItem.RavenSeenUserIds = SourceObject.RavenSeenUserIds;
                    if (!string.IsNullOrEmpty(SourceObject.RavenViewMode))
                        threadItem.RavenViewMode = (InstaViewMode)Enum.Parse(typeof(InstaViewMode), SourceObject.RavenViewMode, true);

                    threadItem.RavenReplayChainCount = SourceObject.RavenReplayChainCount ?? 0;
                    threadItem.RavenSeenCount = SourceObject.RavenSeenCount;
                    if (SourceObject.RavenExpiringMediaActionSummary != null)
                    {
                        var ravenType = SourceObject.RavenExpiringMediaActionSummary.Type.ToLower() == "raven_delivered" ? InstaRavenType.Delivered : InstaRavenType.Opened;
                        threadItem.RavenExpiringMediaActionSummary = new InstaRavenMediaActionSummary
                        {
                            Count = SourceObject.RavenExpiringMediaActionSummary.Count,
                            Type = ravenType
                        };
                        if (!string.IsNullOrEmpty(SourceObject.RavenExpiringMediaActionSummary.TimeStamp))
                            threadItem.RavenExpiringMediaActionSummary.
                                ExpireTime = DateTimeHelper.UnixTimestampMicrosecondsToDateTime(SourceObject.RavenExpiringMediaActionSummary.TimeStamp);

                    }

                    break;
                }
                case InstaDirectThreadItemType.RavenMedia when SourceObject.VisualMedia != null:
                    threadItem.VisualMedia = ConvertersFabric.Instance.GetVisualMediaContainerConverter(SourceObject.VisualMedia).Convert();
                    break;
                case InstaDirectThreadItemType.ActionLog when SourceObject.ActionLogMedia != null:
                    threadItem.ActionLog = new InstaActionLog
                    {
                        Description = SourceObject.ActionLogMedia.Description
                    };
                    break;
                case InstaDirectThreadItemType.Profile when SourceObject.ProfileMedia != null:
                {
                    var converter = ConvertersFabric.Instance.GetUserShortConverter(SourceObject.ProfileMedia);
                    threadItem.ProfileMedia = converter.Convert();
                    if (SourceObject.ProfileMediasPreview != null && SourceObject.ProfileMediasPreview.Any())
                    {
                        try
                        {
                            var previewMedias = new List<InstaMedia>();
                            foreach (var item in SourceObject.ProfileMediasPreview)
                                previewMedias.Add(ConvertersFabric.Instance.GetSingleMediaConverter(item).Convert());

                            threadItem.ProfileMediasPreview = previewMedias;
                        }
                        catch { }
                    }

                    break;
                }
                case InstaDirectThreadItemType.Placeholder when SourceObject.Placeholder != null:
                    threadItem.Placeholder = new InstaPlaceholder
                    {
                        IsLinked = SourceObject.Placeholder.IsLinked,
                        Message = SourceObject.Placeholder.Message
                    };
                    break;
                case InstaDirectThreadItemType.Location when SourceObject.LocationMedia != null:
                    try
                    {
                        threadItem.LocationMedia = new InstaLocation();
                        if (!string.IsNullOrEmpty(SourceObject.LocationMedia.Address))
                            threadItem.LocationMedia.Address = SourceObject.LocationMedia.Address;

                        if (!string.IsNullOrEmpty(SourceObject.LocationMedia.City))
                            threadItem.LocationMedia.City = SourceObject.LocationMedia.City;

                        if (!string.IsNullOrEmpty(SourceObject.LocationMedia.ExternalId))
                            threadItem.LocationMedia.ExternalId = SourceObject.LocationMedia.ExternalId;

                        if (!string.IsNullOrEmpty(SourceObject.LocationMedia.ExternalIdSource))
                            threadItem.LocationMedia.ExternalSource = SourceObject.LocationMedia.ExternalIdSource;

                        if (!string.IsNullOrEmpty(SourceObject.LocationMedia.ShortName))
                            threadItem.LocationMedia.ShortName = SourceObject.LocationMedia.ShortName;

                        if (!string.IsNullOrEmpty(SourceObject.LocationMedia.Name))
                            threadItem.LocationMedia.Name = SourceObject.LocationMedia.Name;


                        threadItem.LocationMedia.FacebookPlacesId = SourceObject.LocationMedia.FacebookPlacesId;
                        threadItem.LocationMedia.Lat = SourceObject.LocationMedia.Lat;
                        threadItem.LocationMedia.Lng = SourceObject.LocationMedia.Lng;
                    }
                    catch { }

                    break;
                case InstaDirectThreadItemType.FelixShare when SourceObject.FelixShareMedia != null && SourceObject.FelixShareMedia.Video != null:
                    try
                    {
                        threadItem.FelixShareMedia = ConvertersFabric.Instance.GetSingleMediaConverter(SourceObject.FelixShareMedia.Video).Convert();
                    }
                    catch { }

                    break;
                case InstaDirectThreadItemType.ReelShare when SourceObject.ReelShareMedia != null:
                    try
                    {
                        threadItem.ReelShareMedia = ConvertersFabric.Instance.GetReelShareConverter(SourceObject.ReelShareMedia).Convert();
                    }
                    catch { }

                    break;
                case InstaDirectThreadItemType.VoiceMedia when SourceObject.VoiceMedia != null:
                    try
                    {
                        threadItem.VoiceMedia = ConvertersFabric.Instance.GetVoiceMediaConverter(SourceObject.VoiceMedia).Convert();
                    }
                    catch { }

                    break;
                case InstaDirectThreadItemType.AnimatedMedia when SourceObject.AnimatedMedia != null:
                    try
                    {
                        threadItem.AnimatedMedia = ConvertersFabric.Instance.GetAnimatedImageConverter(SourceObject.AnimatedMedia).Convert();
                    }
                    catch { }

                    break;
                case InstaDirectThreadItemType.Hashtag when SourceObject.HashtagMedia != null:
                    try
                    {
                        threadItem.HashtagMedia = ConvertersFabric.Instance.GetDirectHashtagConverter(SourceObject.HashtagMedia).Convert();
                        threadItem.Text = "#" + threadItem.HashtagMedia.Name;
                    }
                    catch { }

                    break;
                case InstaDirectThreadItemType.LiveViewerInvite when SourceObject.LiveViewerInvite != null:
                    try
                    {
                        threadItem.LiveViewerInvite = ConvertersFabric.Instance.GetDirectBroadcastConverter(SourceObject.LiveViewerInvite).Convert();
                    }
                    catch { }

                    break;
            }
            return threadItem;
        }
    }
}