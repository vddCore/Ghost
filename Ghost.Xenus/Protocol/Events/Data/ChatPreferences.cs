using Newtonsoft.Json;

namespace Ghost.Xenus.Protocol.Events.Data
{
    public class ChatPreferences
    {
        [JsonProperty("channel")]
        public string Channel { get; internal set; } = "main";

        [JsonProperty("myself")]
        public UserDescription Myself { get; internal set; } = new UserDescription();

        [JsonProperty("preferences")]
        public UserDescription LookingFor { get; internal set; } = new UserDescription();
    }
}
