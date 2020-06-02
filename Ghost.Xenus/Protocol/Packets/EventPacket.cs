using Ghost.Xenus.Protocol.Events;
using Newtonsoft.Json;

namespace Ghost.Xenus.Protocol.Packets
{
    public class EventPacket : Packet
    {
        public Event Event { get; }

        public EventPacket()
        {
        }

        public EventPacket(string eventName, object eventData)
            => Event = new Event(eventName, eventData);

        public EventPacket(string eventName, object eventData, ulong ceid)
            => Event = new Event(eventName, eventData, ceid);

        public EventPacket(Event ev)
            => Event = ev;

        public EventPacket(Packet other)
            : base(other)
        {
            Event = JsonConvert.DeserializeObject<Event>(JsonData);   
        }

        public string ToServerResponseString()
            => $"4{JsonConvert.SerializeObject(Event)}";
    }
}
