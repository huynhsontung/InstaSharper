using System.Collections.Generic;
using InstaSharper.Classes.ResponseWrappers.BaseResponse;
using Newtonsoft.Json;

namespace InstaSharper.Classes.ResponseWrappers.Errors
{
    public class BadStatusErrorsResponseRecovery : BaseStatusResponse
    {
        [JsonProperty("errors")] public MessageErrorsResponsePhone PhoneNumber { get; set; }
    }

    public class MessageErrorsResponsePhone
    {
        [JsonProperty("phone_number")] public List<string> Errors { get; set; }
    }

    public class MessageErrorsResponseRecoveryEmail : BaseStatusResponse
    {
        [JsonProperty("message")] public string Message { get; set; }
    }
}
