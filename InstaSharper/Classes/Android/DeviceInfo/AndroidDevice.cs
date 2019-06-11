using System;

namespace InstaSharper.Classes.Android.DeviceInfo
{
    [Serializable]
    public class AndroidDevice
    {
        public string DeviceId { get; set; } // format: android-{md5}
        public Guid PhoneId { get; set; } = Guid.NewGuid();
        public Guid Uuid { get; set; } = Guid.NewGuid();
        public Guid GoogleAdId { get; set; } = Guid.NewGuid();
        public Guid RankToken { get; set; } = Guid.NewGuid();

        public string UserAgentString { get; set; }
        public AndroidVersion AndroidVersion { get; set; }
        public int Dpi { get; set; }
        public Resolution ScreenResolution = new Resolution();
        public string DeviceName { get; set; }
        public string Cpu { get; set; }
        public string HardwareManufacturer { get; set; }
        public string HardwareModel { get; set; }

        public struct Resolution
        {
            public int Width { get; set; }
            public int Height { get; set; }
        }

    }
}