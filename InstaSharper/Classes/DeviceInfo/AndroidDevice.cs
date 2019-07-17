using System;

namespace InstaSharper.Classes.DeviceInfo
{
    [Serializable]
    public class AndroidDevice
    {
        public string DeviceId { get; internal set; } // format: android-{md5}
        public Guid PhoneId { get; internal set; } = Guid.NewGuid();
        public Guid Uuid { get; internal set; } = Guid.NewGuid();
        public Guid GoogleAdId { get; internal set; } = Guid.NewGuid();
        public Guid RankToken { get; internal set; } = Guid.NewGuid();
        public Guid AdId { get; internal set; } = Guid.NewGuid();

        public string UserAgent { get; internal set; }
        public AndroidVersion AndroidVersion { get; internal set; }
        public int Dpi { get; internal set; }
        public Resolution ScreenResolution = new Resolution();
        public string DeviceName { get; internal set; }
        public string Cpu { get; internal set; }
        public string HardwareManufacturer { get; internal set; }
        public string HardwareModel { get; internal set; }

        internal const string CPU_ABI = "armeabi-v7a:armeabi";
    }

    [Serializable]
    public struct Resolution
    {
        public int Width { get; internal set; }
        public int Height { get; internal set; }

        public override string ToString()
        {
            return $"{Width}x{Height}";
        }
    }
}