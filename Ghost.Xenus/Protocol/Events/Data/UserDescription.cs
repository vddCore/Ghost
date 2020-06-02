using Newtonsoft.Json;

namespace Ghost.Xenus.Protocol.Events.Data
{
    public class UserDescription
    {
        [JsonProperty("sex")]
        public int Sex { get; internal set; }

        [JsonProperty("loc")]
        public int Location { get; internal set; }
    }
}
