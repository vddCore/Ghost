namespace Ghost.Xenus
{
    public class RawDataEventArgs
    {
        public string RawPacket { get; }

        internal RawDataEventArgs(string rawPacket)
            => RawPacket = rawPacket;
    }
}
