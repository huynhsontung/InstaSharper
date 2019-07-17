/*
 * Developer: Ramtin Jokar [ Ramtinak@live.com ] [ My Telegram Account: https://t.me/ramtinak ]
 * 
 * Github source: https://github.com/ramtinak/InstagramApiSharp
 * Nuget package: https://www.nuget.org/packages/InstagramApiSharp
 * 
 * IRANIAN DEVELOPERS
 */

using InstaSharper.Classes.Models.Media;

namespace InstaSharper.Classes.Models.Hashtags
{
    public class InstaDirectHashtag
    {
        public string Name { get; set; }

        public long MediaCount { get; set; }

        public InstaMedia Media { get; set; }
    }
}
