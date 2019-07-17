/*
 * Developer: Ramtin Jokar [ Ramtinak@live.com ] [ My Telegram Account: https://t.me/ramtinak ]
 * 
 * Github source: https://github.com/ramtinak/InstagramApiSharp
 * Nuget package: https://www.nuget.org/packages/InstagramApiSharp
 * 
 * IRANIAN DEVELOPERS
 */

using InstaSharper.Classes.ResponseWrappers.Media;
using Newtonsoft.Json;

namespace InstaSharper.Classes.ResponseWrappers.Direct
{
    public class InstaFelixShareResponse
    {
        [JsonProperty("video")] public InstaMediaItemResponse Video { get; set; }
    }
}
