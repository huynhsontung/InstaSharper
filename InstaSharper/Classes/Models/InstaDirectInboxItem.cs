using System;

namespace InstaSharper.Classes.Models
{
    public class InstaDirectInboxItem : IEquatable<InstaDirectInboxItem>
    {
        public string Text { get; set; }

        public string UserId { get; set; }


        public DateTime TimeStamp { get; set; }


        public string ItemId { get; set; }


        public InstaDirectThreadItemType ItemType { get; set; } = InstaDirectThreadItemType.Unsupported;

        public InstaInboxMedia Media { get; set; }

        public InstaMedia MediaShare { get; set; }

        public Guid ClientContext { get; set; }

        public string UnsupportedType { get; set; }

        public bool FromMe { get; set; } = false;

        public bool Equals(InstaDirectInboxItem other)
        {
            return ItemId == other.ItemId;
        }
    }
}