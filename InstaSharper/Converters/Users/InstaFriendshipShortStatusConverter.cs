/*
 * Developer: Ramtin Jokar [ Ramtinak@live.com ] [ My Telegram Account: https://t.me/ramtinak ]
 * 
 * Github source: https://github.com/ramtinak/InstagramApiSharp
 * Nuget package: https://www.nuget.org/packages/InstagramApiSharp
 * 
 * IRANIAN DEVELOPERS
 */

using System;
using InstaSharper.Classes.Models.User;
using InstaSharper.Classes.ResponseWrappers.User;

namespace InstaSharper.Converters.Users
{
    internal class InstaFriendshipShortStatusConverter :
        IObjectConverter<InstaFriendshipShortStatus, InstaFriendshipShortStatusResponse>
    {
        public InstaFriendshipShortStatusResponse SourceObject { get; set; }

        public InstaFriendshipShortStatus Convert()
        {
            if (SourceObject == null) throw new ArgumentNullException($"Source object");

            var friendships = new InstaFriendshipShortStatus
            {
                Following = SourceObject.Following,
                IncomingRequest = SourceObject.IncomingRequest,
                IsBestie = SourceObject.IsBestie,
                IsPrivate = SourceObject.IsPrivate,
                OutgoingRequest = SourceObject.OutgoingRequest
            };

            return friendships;
        }
    }
}
