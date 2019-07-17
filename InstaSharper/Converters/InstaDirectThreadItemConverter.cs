using System;
using InstaSharper.Classes.Models;
using InstaSharper.Classes.ResponseWrappers;
using InstaSharper.Helpers;

namespace InstaSharper.Converters
{
    internal class InstaDirectThreadItemConverter : IObjectConverter<InstaDirectInboxItem, InstaDirectInboxItemResponse>
    {
        public InstaDirectInboxItemResponse SourceObject { get; set; }

        public string ViewerId { get; set; }

        public InstaDirectInboxItem Convert()
        {
            var threadItem = new InstaDirectInboxItem
            {
                ClientContext = SourceObject.ClientContext,
                ItemId = SourceObject.ItemId,
                TimeStamp = DateTimeHelper.UnixTimestampMilisecondsToDateTime(SourceObject.TimeStamp),
                UserId = SourceObject.UserId
            };

            threadItem.FromMe = threadItem.UserId == ViewerId ? true : false;

            var truncatedItemType = SourceObject.ItemType.Trim().Replace("_", "");
            if (Enum.TryParse(truncatedItemType, true, out InstaDirectThreadItemType type))
                threadItem.ItemType = type;

            switch (threadItem.ItemType)
            {
                case InstaDirectThreadItemType.Link:
                    threadItem.Text = SourceObject.Link?.LinkContext?.LinkUrl;
                    break;
                case InstaDirectThreadItemType.Like:
                    threadItem.Text = SourceObject.Like;
                    break;
                case InstaDirectThreadItemType.Media:
                    if(SourceObject.Media != null)
                    {
                        var converter = ConvertersFabric.Instance.GetInboxMediaConverter(SourceObject.Media);
                        threadItem.Media = converter.Convert();
                    }
                    break;
                case InstaDirectThreadItemType.MediaShare:
                    if (SourceObject.MediaShare != null)
                    {
                        var converter = ConvertersFabric.Instance.GetSingleMediaConverter(SourceObject.MediaShare);
                        threadItem.MediaShare = converter.Convert();
                    }
                    break;
                case InstaDirectThreadItemType.Text:
                    threadItem.Text = SourceObject.Text;
                    break;
                default:
                    threadItem.UnsupportedType = SourceObject.ItemType;
                    break;
            }

            return threadItem;
        }
    }
}