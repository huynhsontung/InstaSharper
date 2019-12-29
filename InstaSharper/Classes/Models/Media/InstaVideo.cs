using InstaSharper.Classes.ResponseWrappers.Media;
using Newtonsoft.Json;

namespace InstaSharper.Classes.Models.Media
{
    public class InstaVideo : InstaVideoResponse
    {
        public InstaVideo() { }
        public InstaVideo(string url, int width, int height) : this(url, width, height, 3) { }
        public InstaVideo(string url, int width, int height, int type)
        {
            Url = url;
            Width = width;
            Height = height;
            Type = type;
        }

        internal string UploadId { get; set; }

        public double Length { get; set; } = 0;

        [JsonIgnore]
        /// <summary>
        /// This is only for .NET core apps like UWP(Windows 10) apps
        /// </summary>
        public byte[] VideoBytes { get; set; }
    }
}