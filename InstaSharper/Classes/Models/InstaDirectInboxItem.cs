using System;

namespace InstaSharper.Classes.Models
{
    public class InstaDirectInboxItem
    {
        public string Text { get; set; }

        public long UserId { get; set; }


        public DateTime TimeStamp { get; set; }


        public string ItemId { get; set; }


        public InstaDirectThreadItemType ItemType { get; set; } = InstaDirectThreadItemType.Unsupported;

        public InstaInboxMedia Media { get; set; }

        public InstaMedia MediaShare { get; set; }

        public Guid ClientContext { get; set; }

        public string UnsupportedType { get; set; }
    }
}