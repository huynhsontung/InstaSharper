using InstaSharper.Classes.ResponseWrappers.BaseResponse;
using Newtonsoft.Json;

namespace InstaSharper.Classes.ResponseWrappers.Errors
{
    public class BadStatusErrorsResponse : BaseStatusResponse
    {
        [JsonProperty("message")] public MessageErrorsResponse Message { get; set; }
    }
}