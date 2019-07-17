using InstaSharper.Classes.ResponseWrappers.BaseResponse;
using Newtonsoft.Json;

namespace InstaSharper.Classes.ResponseWrappers.Media
{
    public class DeleteResponse : BaseStatusResponse
    {
        [JsonProperty("did_delete")] public bool IsDeleted { get; set; }
    }
}