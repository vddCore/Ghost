using Newtonsoft.Json;

namespace Ghost.Xenus.Protocol.Events.Data
{
    public class Captcha
    {
        [JsonProperty("tlce", NullValueHandling = NullValueHandling.Ignore)]
        public CaptchaData Data { get; internal set; }
    }
}