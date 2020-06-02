using Newtonsoft.Json;

namespace Ghost.Xenus.Protocol.Events.Data
{
    public class ChatEndInfo
    {
        [JsonProperty("ckey")]
        public string Key { get; internal set; }

        public ChatEndInfo(string key)
        {
            Key = key;
        }
    }
}
