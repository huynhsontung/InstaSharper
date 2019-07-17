using System;
using InstaSharper.Classes.Models.User;
using InstaSharper.Classes.ResponseWrappers.User;
using InstaSharper.Helpers;

namespace InstaSharper.Converters.Users
{
    internal class InstaSingleUserPresenceConverter : IObjectConverter<InstaUserPresence, InstaUserPresenceResponse>
    {
        public InstaUserPresenceResponse SourceObject { get; set; }

        public InstaUserPresence Convert()
        {
            if (SourceObject == null) throw new ArgumentNullException($"Source object");
            var userPresence = new InstaUserPresence
            {
                Pk = SourceObject.Pk,
                IsActive = SourceObject.IsActive ?? false,
                LastActivity = DateTimeHelper.FromUnixTimeMiliSeconds(SourceObject.LastActivityAtMs ?? 0)
            };
            return userPresence;
        }
    }
}
