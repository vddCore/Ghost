using Newtonsoft.Json;

namespace Ghost.Xenus.Protocol.Events.Data
{
    public class ChatStartedAck
    {
        [JsonProperty("ckey")]
        public string Key { get; internal set; }
    }
}
