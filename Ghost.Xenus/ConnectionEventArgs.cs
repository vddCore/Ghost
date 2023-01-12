using Ghost.Xenus.Protocol.Events.Data;

namespace Ghost.Xenus
{
    public class ConnectionEventArgs
    {
        public ConnectionInfo ConnectionInfo { get; }

        internal ConnectionEventArgs(ConnectionInfo connectionInfo)
            => ConnectionInfo = connectionInfo;
    }
}
