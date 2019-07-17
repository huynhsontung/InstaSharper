/*
 * Developer: Ramtin Jokar [ Ramtinak@live.com ] [ My Telegram Account: https://t.me/ramtinak ]
 * 
 * Github source: https://github.com/ramtinak/InstagramApiSharp
 * Nuget package: https://www.nuget.org/packages/InstagramApiSharp
 * 
 * IRANIAN DEVELOPERS
 */

using System;
using InstaSharper.Classes.Models.Broadcast;
using InstaSharper.Classes.ResponseWrappers.Broadcast;
using InstaSharper.Helpers;

namespace InstaSharper.Converters.Broadcast
{
    internal class InstaBroadcastCommentConverter : IObjectConverter<InstaBroadcastComment, InstaBroadcastCommentResponse>
    {
        public InstaBroadcastCommentResponse SourceObject { get; set; }

        public InstaBroadcastComment Convert()
        {
            if (SourceObject == null) throw new ArgumentNullException($"Source object");
            var broadcastComment = new InstaBroadcastComment
            {
                MediaId = SourceObject.MediaId,
                ContentType = SourceObject.ContentType,
                CreatedAt = DateTimeHelper.FromUnixTimeSeconds(SourceObject.CreatedAt ?? DateTime.Now.ToUnixTime()),
                CreatedAtUtc = DateTimeHelper.FromUnixTimeSeconds(SourceObject.CreatedAtUtc ?? DateTime.UtcNow.ToUnixTime()),
                Pk = SourceObject.Pk,
                Text = SourceObject.Text,
                Type = SourceObject.Type,
                BitFlags = SourceObject.BitFlags,
                DidReportAsSpam = SourceObject.DidReportAsSpam,
                InlineComposerDisplayCondition = SourceObject.InlineComposerDisplayCondition,
                UserId = SourceObject.UserId
            };
            if (SourceObject.User != null)
                broadcastComment.User = ConvertersFabric.Instance
                    .GetUserShortFriendshipFullConverter(SourceObject.User).Convert();

            return broadcastComment;
        }
    }
}
