using Newtonsoft.Json;

namespace Ghost.Xenus.Protocol.Events.Data
{
    public class ClientInfo
    {
        [JsonProperty("hash")]
        public string Hash { get; internal set; }

        [JsonProperty("dpa")]
        public bool AcceptedTermsOfService { get; internal set; } = true;

        [JsonProperty("caper")]
        public bool RequestCaptcha { get; internal set; }
    }
}
