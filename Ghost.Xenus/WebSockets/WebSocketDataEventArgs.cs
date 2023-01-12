namespace Ghost.Xenus.WebSockets
{
    public class WebSocketDataEventArgs
    {
        public string Data { get; }

        internal WebSocketDataEventArgs(string data)
        {
            Data = data;
        }
    }
}