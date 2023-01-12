using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Ghost.Xenus.Protocol.Events
{
    public class Event
    {
        [JsonProperty("ev_name")]
        public string Name { get; set; }

        [JsonProperty("ev_data", NullValueHandling = NullValueHandling.Ignore)]
        public JToken Data { get; set; }

        [JsonProperty("ceid", NullValueHandling = NullValueHandling.Ignore)]
        public ulong? ClientEventID { get; }

        public Event() { }

        public Event(string name, object data)
        {
            Name = name;

            if (data != null)
                Data = JToken.FromObject(data);
        }

        public Event(string name, object data, ulong ceid)
            : this(name, data)
        {
            ClientEventID = ceid;
        }

        public T DataAs<T>()
            => Data.ToObject<T>();
    }
}
