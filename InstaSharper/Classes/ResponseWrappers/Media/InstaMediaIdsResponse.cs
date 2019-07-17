/*
 * Developer: Ramtin Jokar [ Ramtinak@live.com ] [ My Telegram Account: https://t.me/ramtinak ]
 * 
 * Github source: https://github.com/ramtinak/InstagramApiSharp
 * Nuget package: https://www.nuget.org/packages/InstagramApiSharp
 * 
 * IRANIAN DEVELOPERS
 */

using InstaSharper.Classes.Models.Media;
using InstaSharper.Classes.Models.Other;
using Newtonsoft.Json;

namespace InstaSharper.Classes.ResponseWrappers.Media
{
    public class InstaMediaIdsResponse : InstaDefault
    {
        [JsonProperty("media_ids")] public InstaMediaIdList MediaIds = new InstaMediaIdList();        
    }
}
