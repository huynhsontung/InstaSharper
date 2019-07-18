using System;
using System.Collections.Generic;
using InstaSharper.Enums;

namespace InstaSharper.Classes.DeviceInfo
{
    [Serializable]
    public class ApiVersion
    {
        /// <summary>
        ///     This property has data for signed requests. Changing this leads to changes in request handling of all the processors.
        /// </summary>
        public static ApiVersion CurrentApiVersion = GetApiVersion(ApiVersionNumber.Version86);

        public string SignatureKey { get; set; }
        public string AppVersion { get; set; }
        public string AppVersionCode { get; set; }
        public string Capabilities { get; set; }

        internal static readonly Dictionary<ApiVersionNumber, ApiVersion> ApiVersions =
            new Dictionary<ApiVersionNumber, ApiVersion>
            {
                {
                    ApiVersionNumber.Version35,
                    new ApiVersion
                    {
                        AppVersionCode = "95414346",
                        AppVersion = "35.0.0.20.96",
                        Capabilities = "3brTBw==",
                        SignatureKey = "be01114435207c0a0b11a5cf68faeb82ec4eee37c52e8429af5fff6b54b80b28"
                    }
                },
                {
                    ApiVersionNumber.Version44,
                    new ApiVersion
                    {
                        AppVersionCode = "107092322",
                        AppVersion = "44.0.0.9.93",
                        Capabilities = "3brTPw==",
                        SignatureKey = "25f955cc0c8f080a0592aa1fd2572d60afacd5f3c03090cf47ca409068b0d2e1"
                    }
                },
                {
                    ApiVersionNumber.Version61,
                    new ApiVersion
                    {
                        AppVersionCode = "107092322",
                        AppVersion = "61.0.0.19.86",
                        Capabilities = "3brTPw==",
                        SignatureKey = "25f955cc0c8f080a0592aa1fd2572d60afacd5f3c03090cf47ca409068b0d2e1"
                    }
                },
                {
                    ApiVersionNumber.Version64,
                    new ApiVersion
                    {
                        AppVersionCode = "125398467",
                        AppVersion = "64.0.0.14.96",
                        Capabilities = "3brTvw==",
                        SignatureKey = "ac5f26ee05af3e40a81b94b78d762dc8287bcdd8254fe86d0971b2aded8884a4"
                    }
                },
                {
                    ApiVersionNumber.Version74,
                    new ApiVersion
                    {
                        AppVersionCode = "125398467",
                        AppVersion = "74.0.0.21.99",
                        Capabilities = "3brTvw==",
                        SignatureKey = "ac5f26ee05af3e40a81b94b78d762dc8287bcdd8254fe86d0971b2aded8884a4"
                    }
                },
                {
                    ApiVersionNumber.Version76,
                    new ApiVersion
                    {
                        AppVersionCode = "138226743",
                        AppVersion = "76.0.0.15.395",
                        Capabilities = "3brTvw==",
                        SignatureKey = "19ce5f445dbfd9d29c59dc2a78c616a7fc090a8e018b9267bc4240a30244c53b"
                    }
                },
                {
                    ApiVersionNumber.Version86,
                    new ApiVersion
                    {
                        AppVersionCode = "147375143",
                        AppVersion = "86.0.0.24.87",
                        Capabilities = "3brTvw==",
                        SignatureKey = "19ce5f445dbfd9d29c59dc2a78c616a7fc090a8e018b9267bc4240a30244c53b"
                    }
                }
            };

        public static ApiVersion GetApiVersion(ApiVersionNumber versionNumber)
        {
            return ApiVersions[versionNumber];
        }

    }
}
