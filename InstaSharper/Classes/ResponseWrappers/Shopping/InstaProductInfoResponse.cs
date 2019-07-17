/*
 * Developer: Ramtin Jokar [ Ramtinak@live.com ] [ My Telegram Account: https://t.me/ramtinak ]
 * 
 * Github source: https://github.com/ramtinak/InstagramApiSharp
 * Nuget package: https://www.nuget.org/packages/InstagramApiSharp
 * 
 * IRANIAN DEVELOPERS
 */

using System.Collections.Generic;
using InstaSharper.Classes.Models.Other;
using InstaSharper.Classes.ResponseWrappers.BaseResponse;
using InstaSharper.Classes.ResponseWrappers.Media;
using InstaSharper.Classes.ResponseWrappers.User;
using Newtonsoft.Json;

namespace InstaSharper.Classes.ResponseWrappers.Shopping
{
    public class InstaProductInfoResponse : InstaDefault
    {
        [JsonProperty("product_item")] public InstaProductResponse Product { get; set; }

        [JsonProperty("other_product_items")] public List<InstaProductResponse> OtherProductItems { get; set; }

        [JsonProperty("user")] public InstaUserShortResponse User { get; set; }

        [JsonProperty("more_from_business")] public InstaProductMediaListResponse MoreFromBusiness { get; set; }
    }

    public class InstaProductMediaListResponse : BaseLoadableResponse
    {
        [JsonProperty("items")] public List<InstaMediaItemResponse> Medias { get; set; } = new List<InstaMediaItemResponse>();
    }

}
