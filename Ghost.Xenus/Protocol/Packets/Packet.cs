using System;

namespace Ghost.Xenus.Protocol.Packets
{
    public class Packet
    {
        public string RawData { get; }

        public PacketType Type { get; private set; }
        public string JsonData { get; private set; }

        public Packet()
        {
        }

        public Packet(string rawData)
        {
            RawData = rawData;
            ParsePacket();
        }

        public Packet(Packet other)
        {
            RawData = new string(other.RawData);
            JsonData = new string(other.JsonData);
            Type = other.Type;
        }

        public T As<T>() where T : Packet, new()
            => Activator.CreateInstance(
                typeof(T),
                new object[] {this}
            ) as T;

        private void ParsePacket()
        {
            var type = RawData[0];

            if (!char.IsDigit(type))
                throw new FormatException("Invalid raw data provided.");

            var num = type - '0';

            Type = (PacketType)num;
            JsonData = RawData.Substring(1);
        }
    }
}