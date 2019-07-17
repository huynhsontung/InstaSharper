using System;
using System.Collections.Generic;
using System.ComponentModel;
using InstaSharper.Classes.Models.User;
using InstaSharper.Enums;

namespace InstaSharper.Classes.Models.Comment
{
    public class InstaComment : INotifyPropertyChanged
    {
        public int Type { get; set; }

        public int BitFlags { get; set; }

        public long UserId { get; set; }

        public string Status { get; set; }

        public DateTime CreatedAtUtc { get; set; }

        public int LikesCount { get; set; }

        public DateTime CreatedAt { get; set; }

        public InstaContentType ContentType { get; set; }
        public InstaUserShort User { get; set; }
        public long Pk { get; set; }
        public string Text { get; set; }

        public bool DidReportAsSpam { get; set; }

        private bool _hasLikedComment;
        public bool HasLikedComment { get => _hasLikedComment; set { _hasLikedComment = value; Update("HasLikedComment"); } }

        public int ChildCommentCount { get; set; }

        //public int NumTailChildComments { get; set; }

        public bool HasMoreTailChildComments { get; set; }

        public bool HasMoreHeadChildComments { get; set; }

        //public string NextMaxChildCursor { get; set; }
        public List<InstaCommentShort> PreviewChildComments { get; set; } = new List<InstaCommentShort>();

        public List<InstaUserShort> OtherPreviewUsers { get; set; } = new List<InstaUserShort>();

        public event PropertyChangedEventHandler PropertyChanged;
        private void Update(string PName) { PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(PName)); }

        public bool Equals(InstaComment comment)
        {
            return Pk == comment?.Pk;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as InstaComment);
        }

        public override int GetHashCode()
        {
            return Pk.GetHashCode();
        }
    }
}