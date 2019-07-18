/*
 * Developer: Ramtin Jokar [ Ramtinak@live.com ] [ My Telegram Account: https://t.me/ramtinak ]
 * 
 * Github source: https://github.com/ramtinak/InstagramApiSharp
 * Nuget package: https://www.nuget.org/packages/InstagramApiSharp
 * 
 * IRANIAN DEVELOPERS
 */

namespace InstaSharper.Enums
{
    public enum ApiVersionNumber
    {
        Latest = 0,
        /// <summary>
        ///     Api version => No more consent required error.
        /// </summary>
        Version35 = 1,
        /// <summary>
        ///     Api version 44.0.0.9.93 => No more consent required error.
        /// </summary>
        Version44 = 2,
        /// <summary>
        ///     Api version 61.0.0.19.86 => All data like signature key, version code and ... is for v44 except instagram version
        /// </summary>
        Version61 = 3,
        /// <summary>
        ///     Api version 64.0.0.14.96
        /// </summary>
        Version64 = 4,
        /// <summary>
        ///     Api version 74.0.0.21.99 => All data like signature key, version code and ... is for v64 except instagram version
        /// </summary>
        Version74 = 5,
        /// <summary>
        ///     Api version 76.0.0.15.395
        /// </summary>
        Version76 = 6,

        /// <summary>
        ///     Api version 85.0.0.21.100 => Worked well with push client
        /// </summary>
        Version85 = 7,

        /// <summary>
        ///     Api version 86.0.0.24.87
        /// </summary>
        Version86 = 8
    }
}
