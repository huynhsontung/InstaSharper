/*
 * Developer: Ramtin Jokar [ Ramtinak@live.com ] [ My Telegram Account: https://t.me/ramtinak ]
 * 
 * Github source: https://github.com/ramtinak/InstagramApiSharp
 * Nuget package: https://www.nuget.org/packages/InstagramApiSharp
 * 
 * IRANIAN DEVELOPERS
 */

namespace InstaSharper.Classes.Models.Direct
{
    public class InstaAnimatedImage
    {
        public string Id { get; set; }

        public InstaAnimatedImageMedia Media { get; set; }

        public bool IsRandom { get; set; }

        public bool IsSticker { get; set; }

        public InstaAnimatedImageUser User { get; set; }
    }

    public class InstaAnimatedImageUser
    {
        public bool IsVerified { get; set; }

        public string Username { get; set; }
    }

    public class InstaAnimatedImageMedia
    {
        public string Url { get; set; }

        public int Width { get; set; } = 0;

        public int Height { get; set; } = 0;

        public int Size { get; set; }

        public string Mp4Url { get; set; }

        public int Mp4Size { get; set; }

        public string WebpUrl { get; set; }

        public int WebpSize { get; set; }
    }
}
