using System.Collections.Generic;
using InstaSharper.Classes.Models;
using InstaSharper.Classes.ResponseWrappers;
using InstaSharper.Helpers;

namespace InstaSharper.Converters
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
                VieweId = SourceObject.VieweId,
                LastActivity = DateTimeHelper.UnixTimestampMilisecondsToDateTime(SourceObject.LastActivity),
                ThreadId = SourceObject.ThreadId,
                OldestCursor = SourceObject.OldestCursor,
                NewestCursor = SourceObject.NewestCursor,
                NextCursor = SourceObject.NextCursor,
                PrevCursor = SourceObject.PrevCursor,
                ThreadType = SourceObject.ThreadType,
                Title = SourceObject.Title
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
                    thread.Items.Add(converter.Convert());
                }
            }

            if(SourceObject.LastPermanentItem != null)
            {
                var converter = ConvertersFabric.Instance.GetDirectThreadItemConverter(SourceObject.LastPermanentItem);
                thread.LastPermanentItem = converter.Convert();
            }

            if (SourceObject.Users != null && SourceObject.Users.Count > 0)
            {
                thread.Users = new InstaUserShortList();
                foreach (var user in SourceObject.Users)
                {
                    var converter = ConvertersFabric.Instance.GetUserShortConverter(user);
                    thread.Users.Add(converter.Convert());
                }
            }

            return thread;
        }
    }
}