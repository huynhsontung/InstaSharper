/*
 * Developer: Ramtin Jokar [ Ramtinak@live.com ] [ My Telegram Account: https://t.me/ramtinak ]
 * 
 * Github source: https://github.com/ramtinak/InstagramApiSharp
 * Nuget package: https://www.nuget.org/packages/InstagramApiSharp
 * 
 * IRANIAN DEVELOPERS
 */

using System;
using InstaSharper.Classes.Models.Direct;
using InstaSharper.Classes.ResponseWrappers.Direct;

namespace InstaSharper.Converters.Directs
{
    internal class InstaAnimatedImageUserConverter : IObjectConverter<InstaAnimatedImageUser, InstaAnimatedImageUserResponse>
    {
        public InstaAnimatedImageUserResponse SourceObject { get; set; }

        public InstaAnimatedImageUser Convert()
        {
            if (SourceObject == null) throw new ArgumentNullException($"Source object");

            var user = new InstaAnimatedImageUser
            {
                IsVerified = SourceObject.IsVerified,
                Username = SourceObject.Username
            };

            return user;
        }
    }
}
