/*
 * Developer: Ramtin Jokar [ Ramtinak@live.com ] [ My Telegram Account: https://t.me/ramtinak ]
 * 
 * Github source: https://github.com/ramtinak/InstagramApiSharp
 * Nuget package: https://www.nuget.org/packages/InstagramApiSharp
 * 
 * IRANIAN DEVELOPERS
 */

using System;
using InstaSharper.Classes.Models.Other;
using InstaSharper.Classes.ResponseWrappers.Other;

namespace InstaSharper.Converters.Other
{
    internal class InstaPresenceConverter : IObjectConverter<InstaPresence, InstaPresenceResponse>
    {
        public InstaPresenceResponse SourceObject { get; set; }

        public InstaPresence Convert()
        {
            if (SourceObject == null) throw new ArgumentNullException("SourceObject");

            var presence = new InstaPresence
            {
                PresenceDisabled = SourceObject.Disabled ?? false,
                ThreadPresenceDisabled = SourceObject.ThreadPresenceDisabled ?? false
            };

            return presence;
        }
    }
}
