using System.Collections.Generic;
using Newtonsoft.Json;

namespace InstaSharper.Classes.ResponseWrappers.Errors
{
    public class MessageErrorsResponse
    {
        [JsonProperty("errors")] public List<string> Errors { get; set; }
    }
}