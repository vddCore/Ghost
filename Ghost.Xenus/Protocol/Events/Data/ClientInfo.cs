using Newtonsoft.Json;

namespace Ghost.Xenus.Protocol.Events.Data
{
    public class ClientInfo
    {
        public class ClientTestData
        {
            [JsonProperty("ckey")]
            public int ClientKey { get; internal set; }

            [JsonProperty("recevsent")]
            public bool WasReconnectEventSent { get; internal set; }
        }

        [JsonProperty("cvdate")]
        public string ClientVersionDate { get; internal set; } = "2017-08-01";

        [JsonProperty("mobile")]
        public bool IsMobile { get; internal set; }

        [JsonProperty("cver")]
        public string ClientVersion { get; internal set; } = "v2.5";

        [JsonProperty("adf")]
        public string AdvertisementFormat { get; internal set; } = "ajaxPHP";

        [JsonProperty("hash")]
        public string Hash { get; internal set; }

        [JsonProperty("testdata")]
        public ClientTestData TestData { get; internal set; } = new ClientTestData();
    }
}
