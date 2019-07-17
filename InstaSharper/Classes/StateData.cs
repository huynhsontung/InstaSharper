using System;
using System.Net;
using InstaSharper.API.Push;
using InstaSharper.Classes.DeviceInfo;
using InstaSharper.Enums;

namespace InstaSharper.Classes
{
    [Serializable]
    internal class StateData
    {
        public AndroidDevice DeviceInfo { get; set; }
        public UserSessionData UserSession { get; set; }
        public bool IsAuthenticated { get; set; }
        public CookieContainer Cookies { get; set; }
        public FbnsConnectionData FbnsConnectionData { get; set; }
        public ApiVersionNumber? ApiVersion { get; set; }
    }
}