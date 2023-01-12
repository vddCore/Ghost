using Ghost.Xenus.Protocol;
using Ghost.Xenus.Protocol.Events.Data;
using Ghost.Xenus.Protocol.Handling;
using Ghost.Xenus.Protocol.Packets;
using Ghost.Xenus.Rest;
using Ghost.Xenus.WebSockets;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Timers;

namespace Ghost.Xenus
{
    public class Client
    {
        public const string WebServiceAddress = "https://6obcy.org";
        public const string TalkServiceAddress = "https://6obcy.org/rozmowa";

        private ulong _clientEventId = 1;
        internal ulong ClientEventID => _clientEventId++;

        private RestClient Rest { get; }
        private EventWebSocket WebSocket { get; }

        private Dispatcher Dispatcher { get; set; }
        private Timer KeepAliveTimer { get; set; }

        private Stack<bool> SearchRequests { get; }

        public bool IsConnectionOpen => WebSocket.IsOpen;

        public bool IsCurrentlyChatting => ActiveChats.Count > 0;
        public bool IsSearchingForChat => SearchRequests.Peek();

        public ConnectionData ConnectionData { get; }
        public string SessionID { get; private set; }

        public ConnectionInfo ConnectionInfo { get; internal set; }
        public List<ChatInfo> ActiveChats { get; internal set; }

        public event EventHandler<RawDataEventArgs> RawPacketSent;
        public event EventHandler<RawDataEventArgs> RawPacketReceived;
        public event EventHandler<UserCountEventArgs> ActiveUserCountChanged;
        public event EventHandler<ConnectionEventArgs> ConnectionAccepted;
        public event EventHandler<ChatEventArgs> ChatStarted;
        public event EventHandler<ChatEventArgs> ChatEnded;
        public event EventHandler<TypingStateEventArgs> TypingStateChanged;
        public event EventHandler<ChatMessageEventArgs> ChatMessageReceived;
        public event EventHandler<ChatMessageEventArgs> ChatMessageSent;
        public event EventHandler<ServiceMessageEventArgs> ServiceMessageReceived;
        public event EventHandler<TopicEventArgs> TopicReceived;
        public event EventHandler SocketOpened;
        public event EventHandler SocketClosed;

        public Client(bool secure)
        {
            Rest = new RestClient(WebServiceAddress);
            ConnectionData = RequestConnectionData();

            Dispatcher = new Dispatcher(this);
            Dispatcher.AddProcessorsFrom(Assembly.GetExecutingAssembly());

            ActiveChats = new List<ChatInfo>();

            SearchRequests = new Stack<bool>();
            SearchRequests.Push(false);

            var protocol = secure ? "wss://" : "ws://";

            WebSocket = new EventWebSocket(
                new Uri($"{protocol}{ConnectionData.Host}:{ConnectionData.PortString}/6eio/?EIO=3&transport=websocket"),
                origin: WebServiceAddress
            );

            WebSocket.DataReceived += WebSocket_DataReceived;
            WebSocket.DataSent += WebSocket_DataSent;
            WebSocket.Closed += WebSocket_Closed;
            WebSocket.Opened += WebSocket_Opened;
        }

        public async Task SendMessage(string messageBody, ChatInfo chat = null)
        {
            EnsureChatActive();

            chat ??= ActiveChats.First();

            var message = new ChatMessage(
                chat.CurrentMessageID,
                messageBody,
                chat.Key
            );

            var pmsgPacket = new EventPacket("_pmsg", message, ClientEventID);
            await SendEventPacket(pmsgPacket);

            OnChatMessageSent(message);
        }

        public async Task RequestRandomTopic(ChatInfo chat = null)
        {
            EnsureChatActive();

            chat ??= ActiveChats.First();

            var topicRequest = new TopicRequest(chat.Key);

            var randtopicPacket = new EventPacket("_randtopic", topicRequest, ClientEventID);
            await SendEventPacket(randtopicPacket);
        }

        public async Task SendTypingState(bool typing, ChatInfo chat = null)
        {
            EnsureChatActive();

            chat ??= ActiveChats.First();

            var typingState = new TypingState(
                chat.Key,
                typing
            );

            var mtypPacket = new EventPacket("_mtyp", typingState);
            await SendEventPacket(mtypPacket);
        }

        public async Task EndChat(ChatInfo chat = null)
        {
            EnsureChatActive();

            chat ??= ActiveChats.First();

            var chatEndInfo = new ChatEndInfo(chat.Key);

            var distalkPacket = new EventPacket("_distalk", chatEndInfo, ClientEventID);
            await SendEventPacket(distalkPacket);
        }

        public async Task StartNewChat(Location where = Location.EntirePoland)
        {
            SearchRequests.Push(true);

            var chatPreferences = new ChatPreferences();

            chatPreferences.Myself.Location =
                chatPreferences.LookingFor.Location =
                    where;

            var sasPacket = new EventPacket("_sas", chatPreferences, ClientEventID);
            await SendEventPacket(sasPacket);
        }

        public async Task Connect()
            => await WebSocket.Open();

        public async Task Disconnect()
            => await WebSocket.Close();

        internal async Task SendPing()
            => await WebSocket.SendMessage($"{(int)PacketType.PingPacket}");

        internal async Task SendEventPacket(EventPacket eventPacket)
            => await WebSocket.SendMessage(eventPacket.ToServerResponseString());

        private ConnectionData RequestConnectionData()
        {
            var response = Rest.Execute(
                new RestRequest(
                    Endpoints.AddressDataEndpoint,
                    Method.GET,
                    DataFormat.Json
                )
            );

            return JsonConvert.DeserializeObject<ConnectionData>(response.Content);
        }

        private void SetUpConnectionDetails(SetupData data)
        {
            SessionID = data.SessionID;
            KeepAliveTimer = new Timer(data.PingInterval);

            KeepAliveTimer.Elapsed += async (sender, e) => await SendPing();
            KeepAliveTimer.Start();
        }

        private void SetUpDisconnectedState()
        {
            ConnectionInfo = null;
            ActiveChats.Clear();
            SessionID = null;

            while (SearchRequests.Count > 1)
                SearchRequests.Pop();

            _clientEventId = 1;

            KeepAliveTimer.Stop();
        }

        private void EnsureChatActive()
        {
            if (!IsCurrentlyChatting)
                throw new InvalidOperationException("You need to be in a chat to do this.");
        }

        private void WebSocket_Opened(object sender, EventArgs e)
        {
            Console.WriteLine("Websocket has been opened.");
        }

        private void WebSocket_DataReceived(object sender, WebSocketDataEventArgs e)
        {
            OnRawPacketReceived(e.Data);

            var packet = new Packet(e.Data);

            if (packet.Type == PacketType.SetupPacket)
            {
                var setupPacket = packet.As<SetupPacket>();
                SetUpConnectionDetails(setupPacket.Data);
            }
            else if (packet.Type == PacketType.EventPacket)
            {
                var eventPacket = packet.As<EventPacket>();
                Dispatcher.Dispatch(eventPacket.Event);
            }
        }

        private void WebSocket_DataSent(object sender, WebSocketDataEventArgs e)
            => OnRawPacketSent(e.Data);

        private void WebSocket_Closed(object sender, EventArgs e)
            => SetUpDisconnectedState();

        internal void OnConnectionAccepted(ConnectionInfo connectionInfo)
            => ConnectionAccepted?.Invoke(this, new ConnectionEventArgs(connectionInfo));

        internal void OnChatStarted(ChatInfo chatInfo)
        {
            SearchRequests.Pop();
            ActiveChats.Add(chatInfo);

            ChatStarted?.Invoke(this, new ChatEventArgs(ChatState.Started, chatInfo));
        }

        internal void OnActiveUserCountChanged(int count)
            => ActiveUserCountChanged?.Invoke(this, new UserCountEventArgs(count));

        internal void OnRawPacketSent(string rawPacket)
            => RawPacketSent?.Invoke(this, new RawDataEventArgs(rawPacket));

        internal void OnRawPacketReceived(string rawPacket)
            => RawPacketReceived?.Invoke(this, new RawDataEventArgs(rawPacket));

        internal void OnChatEnded(ulong chatId)
        {
            var chat = ActiveChats.FirstOrDefault(chat => chat.ID == chatId);

            if (chat != null)
            {
                ActiveChats.Remove(chat);
                ChatEnded?.Invoke(this, new ChatEventArgs(ChatState.Ended, chat));
            }
        }

        internal void OnTypingStateChanged(bool isTyping)
            => TypingStateChanged?.Invoke(this, new TypingStateEventArgs(isTyping));

        internal void OnChatMessageReceived(ChatMessage message)
            => ChatMessageReceived?.Invoke(this, new ChatMessageEventArgs(message));

        internal void OnChatMessageSent(ChatMessage message)
            => ChatMessageSent?.Invoke(this, new ChatMessageEventArgs(message));

        internal void OnServiceMessageReceived(string serviceMessage)
            => ServiceMessageReceived?.Invoke(this, new ServiceMessageEventArgs(serviceMessage));

        internal void OnTopicReceived(Topic topic)
            => TopicReceived?.Invoke(this, new TopicEventArgs(topic));
    }
}