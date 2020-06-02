using Newtonsoft.Json;

namespace Ghost.Xenus.Protocol.Events.Data
{
    public class ConnectionInfo
    {
        [JsonProperty("conn_id")]
        public string ID { get; internal set; }

        [JsonProperty("hash")]
        public string Hash { get; internal set; }
    }
}
