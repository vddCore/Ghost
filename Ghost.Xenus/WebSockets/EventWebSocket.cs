using System;
using System.Diagnostics;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Ghost.Xenus.WebSockets
{
    public class EventWebSocket
    {
        private byte[] _dataBuffer;

        private Uri _uri;
        private ClientWebSocket _underlyingWebSocket;

        public int DataBufferSize { get; }

        public event EventHandler<WebSocketDataEventArgs> DataReceived;
        public event EventHandler<WebSocketDataEventArgs> DataSent;
        public event EventHandler Opened;
        public event EventHandler Closed;

        public EventWebSocket(Uri uri, int dataBufferSize = 16384, string origin = null)
        {
            _uri = uri;
            _underlyingWebSocket = new ClientWebSocket();

            if (!string.IsNullOrEmpty(origin))
                _underlyingWebSocket.Options.SetRequestHeader("Origin", origin);

            DataBufferSize = dataBufferSize;
            _dataBuffer = new byte[DataBufferSize];
        }

        public async Task Open()
        {
            await _underlyingWebSocket.ConnectAsync(_uri, CancellationToken.None);
#pragma warning disable 4014
            Task.Factory.StartNew(ReceiveWhileOpen, TaskCreationOptions.LongRunning);
#pragma warning restore 4014
        }

        public async Task Close()
        {
            await _underlyingWebSocket.CloseAsync(
                WebSocketCloseStatus.NormalClosure, 
                "Closed gracefully.", 
                CancellationToken.None
            );

            Closed?.Invoke(this, EventArgs.Empty);
        }

        public async Task SendMessage(string message)
        {
            if (_underlyingWebSocket.State == WebSocketState.Open)
            {
                var bytes = Encoding.UTF8.GetBytes(message);

                await _underlyingWebSocket.SendAsync(
                    new ArraySegment<byte>(bytes),
                    WebSocketMessageType.Text,
                    true,
                    CancellationToken.None
                );

                DataSent?.Invoke(this, new WebSocketDataEventArgs(message));
            }
            else throw new InvalidOperationException("Cannot send data while websocket is not open.");
        }

        private async Task ReceiveWhileOpen()
        {
            Opened?.Invoke(this, EventArgs.Empty);

            var str = string.Empty;
            while (_underlyingWebSocket.State == WebSocketState.Open)
            {
                var memory = new Memory<byte>(_dataBuffer);

                var response = await _underlyingWebSocket.ReceiveAsync(memory, CancellationToken.None);
                if (response.MessageType == WebSocketMessageType.Text)
                {
                    str += Encoding.UTF8.GetString(_dataBuffer[0..response.Count]).TrimEnd();

                    if (response.EndOfMessage)
                    {
                        DataReceived?.Invoke(
                            this,
                            new WebSocketDataEventArgs(str)
                        );

                        str = string.Empty;
                    }
                }
                else if (response.MessageType == WebSocketMessageType.Close)
                {
                    Closed?.Invoke(this, EventArgs.Empty);
                }
            }
        }
    }
}