using System;
using InstaSharper.Classes.Models.Story;
using InstaSharper.Classes.ResponseWrappers.Story;

namespace InstaSharper.Converters.Stories
{
    internal class InstaStoryQuestionStickerItemConverter : IObjectConverter<InstaStoryQuestionStickerItem, InstaStoryQuestionStickerItemResponse>
    {
        public InstaStoryQuestionStickerItemResponse SourceObject { get; set; }

        public InstaStoryQuestionStickerItem Convert()
        {
            if (SourceObject == null) throw new ArgumentNullException($"Source object");
            return new InstaStoryQuestionStickerItem
            {
                BackgroundColor = SourceObject.BackgroundColor,
                ProfilePicUrl = SourceObject.ProfilePicUrl,
                Question = SourceObject.Question,
                QuestionId = SourceObject.QuestionId,
                QuestionType = SourceObject.QuestionType,
                TextColor = SourceObject.TextColor,
                ViewerCanInteract = SourceObject.ViewerCanInteract
            };

        }
    }
}
