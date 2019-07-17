using Newtonsoft.Json;

namespace InstaSharper.Classes.ResponseWrappers.Direct
{
    public class InstaRankedRecipientsResponse : InstaRecipientsResponse, IInstaRecipientsResponse
    {
        [JsonProperty("ranked_recipients")] public RankedRecipientResponse[] RankedRecipients { get; set; }
    }
}