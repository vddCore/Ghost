using Newtonsoft.Json;

namespace Ghost.Xenus.Protocol.Events.Data
{
    public class TopicRequest
    {
        [JsonProperty("ckey")]
        public string ChatKey { get; }

        public TopicRequest(string chatKey)
            => ChatKey = chatKey;
    }
}
