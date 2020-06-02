using Newtonsoft.Json;

namespace Ghost.Xenus.Protocol.Events.Data
{
    public class ChatMessage
    {
        [JsonProperty("post_id", NullValueHandling = NullValueHandling.Ignore)]
        public ulong? PostID { get; internal set; }

        [JsonProperty("cid", NullValueHandling = NullValueHandling.Ignore)]
        public ulong? ChatID { get; internal set; }

        [JsonProperty("msg")]
        public string Body { get; internal set; }

        [JsonProperty("ckey", NullValueHandling = NullValueHandling.Ignore)]
        public string ChatKey { get; internal set; }

        [JsonProperty("idn", NullValueHandling = NullValueHandling.Ignore)]
        public ulong? ID { get; internal set; }

        [JsonProperty("who", NullValueHandling = NullValueHandling.Ignore)]
        public int? Who { get; internal set; }

        public ChatMessage() { }

        internal ChatMessage(ulong id, string body, string chatKey)
        {
            ID = id;
            Body = body;
            ChatKey = chatKey;
        }
    }
}
