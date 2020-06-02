using Newtonsoft.Json;

namespace Ghost.Xenus.Protocol.Events.Data
{
    public class ChatInfo
    {
        private ulong _currentMessageId;

        [JsonProperty("cid")]
        public ulong ID { get; internal set; }

        [JsonProperty("ckey")]
        public string Key { get; internal set; }

        [JsonProperty("flaged")]
        public bool IsFlagged { get; internal set; }

        [JsonIgnore]
        public ulong CurrentMessageID => _currentMessageId++;
    }
}
