using Newtonsoft.Json;
using System.Collections.Generic;

namespace Ghost.Xenus.Protocol
{
    public class SetupData
    {
        [JsonProperty("sid")]
        public string SessionID { get; internal set; }
        
        [JsonProperty("upgrades")]
        public List<string> Upgrades { get; internal set; }
        
        [JsonProperty("pingInterval")]
        public int PingInterval { get; internal set; }
        
        [JsonProperty("pingTimeout")]
        public int PingTimeout { get; internal set; }
    }
}