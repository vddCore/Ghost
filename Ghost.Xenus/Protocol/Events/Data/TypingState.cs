using Newtonsoft.Json;

namespace Ghost.Xenus.Protocol.Events.Data
{
    public class TypingState
    {
        [JsonProperty("ckey")]
        public string ChatKey { get; internal set; }

        [JsonProperty("val")]
        public bool IsTyping { get; internal set; }

        public TypingState() { }

        public TypingState(string chatKey, bool isTyping)
        {
            ChatKey = chatKey;
            IsTyping = isTyping;
        }
    }
}
