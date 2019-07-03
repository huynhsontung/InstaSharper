using System;
using System.Collections.Generic;
using InstaSharper.Classes.Android.DeviceInfo;

// ReSharper disable StringLiteralTypo

namespace InstaSharper.API.Push
{
    class FbnsUserAgent
    {
        const string FBNS_APPLICATION_NAME = "MQTT";
        const string INSTAGRAM_APPLICATION_NAME = "Instagram";  // for Realtime features

        #region InstaSharper Constants
        /// Duplicate from <see cref="InstaSharper.API.InstaApiConstants"/>
//        private const string IG_VERSION = "85.0.0.21.100";
//        private const string VERSION_CODE = "146536611";
//        private const string PACKAGE_NAME = "com.instagram.android";
        #endregion

        // todo: implement Realtime status like "message seen"
        public static string BuildFbUserAgent(AndroidDevice device, string appName = FBNS_APPLICATION_NAME, string userLocale = "en_US")
        {
            var fields = new Dictionary<string, string>
            {
                {"FBAN", appName},
                {"FBAV", InstaApiConstants.IG_VERSION},
                {"FBBV", InstaApiConstants.VERSION_CODE},
                {"FBDM",
                    $"{{density={Math.Round(device.Dpi / 160f, 1):F1},width={device.ScreenResolution.Width},height={device.ScreenResolution.Height}}}"
                },
                {"FBLC", userLocale},
                {"FBCR", ""},   // We don't have cellular
                {"FBMF", device.HardwareManufacturer},
                {"FBBD", device.HardwareManufacturer},
                {"FBPN", InstaApiConstants.PACKAGE_NAME},
                {"FBDV", device.HardwareModel},
                {"FBSV", device.AndroidVersion.VersionNumber},
                {"FBLR", "0"},  // android.hardware.ram.low
                {"FBBK", "1"},  // Const (at least in 10.12.0)
                {"FBCA", AndroidDevice.CPU_ABI}
            };
            var mergeList = new List<string>();
            foreach (var field in fields)
            {
                mergeList.Add($"{field.Key}/{field.Value}");
            }

            var userAgent = "";
            foreach (var field in mergeList)
            {
                userAgent += field + ';';
            }

            return '[' + userAgent + ']';
        }
    }
}
