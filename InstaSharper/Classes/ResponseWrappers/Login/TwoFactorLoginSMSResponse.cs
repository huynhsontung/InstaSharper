using Newtonsoft.Json;

namespace InstaSharper.Classes.ResponseWrappers.Login
{
    public class TwoFactorLoginSMS
    {
        [JsonProperty("two_factor_required")]
        public bool TwoFactorRequired { get; set; }

        [JsonProperty("two_factor_info")]
        public InstaTwoFactorLogin TwoFactorInfo { get; set; }
    }
}
