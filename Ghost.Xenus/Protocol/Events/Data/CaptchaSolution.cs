using Newtonsoft.Json;

namespace Ghost.Xenus.Protocol.Events.Data
{
    public class CaptchaSolution
    {
        [JsonProperty("solution")]
        public string Solution { get; }

        public CaptchaSolution(string solution)
        {
            Solution = solution;
        }
    }
}