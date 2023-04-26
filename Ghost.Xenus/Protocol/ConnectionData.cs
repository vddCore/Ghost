using Newtonsoft.Json;
using System;

namespace Ghost.Xenus.Protocol
{
    [Serializable]
    public class ConnectionData
    {
        [JsonProperty("host")]
        public string Host { get; internal set; }

        [JsonProperty("port")]
        public string PortString { get; internal set; }

        [JsonProperty("from")]
        public string Referrer { get; internal set; }

        [JsonProperty("s6")]
        public string S6 { get; internal set; }

        public uint Port => uint.Parse(PortString);
    }
}