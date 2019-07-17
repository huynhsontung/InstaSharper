using Newtonsoft.Json;

namespace InstaSharper.Classes.ResponseWrappers.Direct
{
    public class InstaRecentRecipientsResponse : InstaRecipientsResponse, IInstaRecipientsResponse
    {
        [JsonProperty("recent_recipients")] public RankedRecipientResponse[] RankedRecipients { get; set; }
    }
}