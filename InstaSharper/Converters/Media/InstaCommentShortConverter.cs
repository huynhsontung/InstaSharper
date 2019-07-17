using System;
using InstaSharper.Classes.Models.Comment;
using InstaSharper.Classes.ResponseWrappers.Comment;
using InstaSharper.Enums;
using InstaSharper.Helpers;

namespace InstaSharper.Converters.Media
{
    internal class InstaCommentShortConverter : IObjectConverter<InstaCommentShort, InstaCommentShortResponse>
    {
        public InstaCommentShortResponse SourceObject { get; set; }

        public InstaCommentShort Convert()
        {
            if (SourceObject == null)
                return null;
            var shortComment = new InstaCommentShort
            {
                CommentLikeCount = SourceObject.CommentLikeCount,
                ContentType = (InstaContentType)Enum.Parse(typeof(InstaContentType), SourceObject.ContentType, true),
                CreatedAt = DateTimeHelper.UnixTimestampToDateTime(SourceObject.CreatedAt),
                CreatedAtUtc = DateTimeHelper.UnixTimestampToDateTime(SourceObject.CreatedAtUtc),
                Pk = SourceObject.Pk,
                Status = SourceObject.Status,
                Text = SourceObject.Text,
                Type = SourceObject.Type,
                User = ConvertersFabric.Instance.GetUserShortConverter(SourceObject.User).Convert(),
                HasLikedComment = SourceObject.HasLikedComment,
                MediaId = SourceObject.MediaId,
                ParentCommentId = SourceObject.ParentCommentId
            };
            return shortComment;
        }
    }
}
