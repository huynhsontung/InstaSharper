using System;

namespace InstaSharper.Classes.Android.DeviceInfo
{
    [Serializable]
    public class AndroidDevice
    {
        public Guid PhoneGuid { get; set; } //
        public string DeviceId { get; set; } // format: "android-{md5}
        public Guid PhoneId { get; set; } = Guid.NewGuid();
        public Guid Uuid { get; set; } = Guid.NewGuid();
        public Guid GoogleAdId { get; set; } = Guid.NewGuid();
        public Guid RankToken { get; set; } = Guid.NewGuid();


        public string AndroidBoardName { get; set; } //
        public string AndroidBootloader { get; set; } //
        public string DeviceBrand { get; set; } //
        public string DeviceModel { get; set; } //
        public string DeviceModelBoot { get; set; } // 
        public string DeviceModelIdentifier { get; set; } //
        public string FirmwareBrand { get; set; } //
        public string FirmwareFingerprint { get; set; } // need clean up. provides android version number
        public string FirmwareTags { get; set; } //
        public string FirmwareType { get; set; } //
        public string HardwareManufacturer { get; set; }
        public string HardwareModel { get; set; }
    }
}