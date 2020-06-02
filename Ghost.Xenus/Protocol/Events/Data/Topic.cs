using Newtonsoft.Json;

namespace Ghost.Xenus.Protocol.Events.Data
{
    public class Topic
    {
        [JsonProperty("post_id")]
        public ulong PostID { get; internal set; }

        [JsonProperty("cid")]
        public ulong ChatID { get; internal set; }

        [JsonProperty("topic")]
        public string Body { get; internal set; }

        [JsonProperty("who")]
        public int Who { get; internal set; }
    }
}
