using InstaSharper.Classes.Models.User;
using InstaSharper.Classes.ResponseWrappers.User;

namespace InstaSharper.Converters.Users
{
    internal class InstaBlockedUserInfoConverter : IObjectConverter<InstaBlockedUserInfo, InstaBlockedUserInfoResponse>
    {
        public InstaBlockedUserInfoResponse SourceObject { get; set; }

        public InstaBlockedUserInfo Convert()
        {
            return new InstaBlockedUserInfo()
            {
                BlockedAt = SourceObject.BlockedAt,
                FullName = SourceObject.FullName,
                IsPrivate = SourceObject.IsPrivate,
                Pk = SourceObject.Pk,
                ProfilePicture = SourceObject.ProfilePicture,
                UserName = SourceObject.UserName
            };
        }
    }
}
