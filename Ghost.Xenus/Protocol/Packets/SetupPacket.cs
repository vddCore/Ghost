using Newtonsoft.Json;

namespace Ghost.Xenus.Protocol.Packets
{
    public class SetupPacket : Packet
    {
        public SetupData Data { get; private set; }

        public SetupPacket()
        {
        }

        public SetupPacket(Packet other)
            : base(other)
        {
            Data = JsonConvert.DeserializeObject<SetupData>(JsonData);
        }
    }
}