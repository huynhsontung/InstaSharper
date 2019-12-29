using InstaSharper.Classes.ResponseWrappers.Media;
using Newtonsoft.Json;

namespace InstaSharper.Classes.Models.Media
{
    public class InstaImage : ImageResponse
    {
        public InstaImage(string url, int width, int height)
        {
            Url = url;
            Width = width;
            Height = height;
        }

        public InstaImage()
        {
        }

        [JsonIgnore]
        /// <summary>
        /// This is only for .NET core apps like UWP(Windows 10) apps
        /// </summary>
        public byte[] ImageBytes { get; set; }
    }
}