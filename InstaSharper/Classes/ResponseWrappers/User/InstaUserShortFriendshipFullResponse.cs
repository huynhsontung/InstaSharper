/*
 * Developer: Ramtin Jokar [ Ramtinak@live.com ] [ My Telegram Account: https://t.me/ramtinak ]
 * 
 * Github source: https://github.com/ramtinak/InstagramApiSharp
 * Nuget package: https://www.nuget.org/packages/InstagramApiSharp
 * 
 * IRANIAN DEVELOPERS
 */

using Newtonsoft.Json;

namespace InstaSharper.Classes.ResponseWrappers.User
{
    public class InstaUserShortFriendshipFullResponse : InstaUserShortResponse
    {
        [JsonProperty("friendship_status")] public InstaFriendshipFullStatusResponse FriendshipStatus { get; set; }
    }
}
