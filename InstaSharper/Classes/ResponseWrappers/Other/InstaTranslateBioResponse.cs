/*
 * Developer: Ramtin Jokar [ Ramtinak@live.com ] [ My Telegram Account: https://t.me/ramtinak ]
 * 
 * Github source: https://github.com/ramtinak/InstagramApiSharp
 * Nuget package: https://www.nuget.org/packages/InstagramApiSharp
 * 
 * IRANIAN DEVELOPERS
 */

using InstaSharper.Classes.Models.Other;
using Newtonsoft.Json;

namespace InstaSharper.Classes.ResponseWrappers.Other
{
    public class InstaTranslateBioResponse : InstaDefault
    {
        [JsonProperty("translation")] public string Translation { get; set; }
    }
}
