using System;
using Newtonsoft.Json;

namespace Ghost.Xenus.Protocol.Events.Data
{
    public class CaptchaData
    {
        [JsonProperty("dtype")]
        public string DataType { get; internal set; }

        [JsonProperty("data")]
        public string DataBase64 { get; internal set; }

        [JsonProperty("cmdText")]
        public string ServiceMessage { get; internal set; }
        
        [JsonIgnore]
        public byte[] ImageData => Convert.FromBase64String(DataBase64.Split(',')[1]);
    }
}