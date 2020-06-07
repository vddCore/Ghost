using Atlas.UI.Controls;
using DevExpress.Mvvm;
using DevExpress.Mvvm.DataAnnotations;
using Ghost.Model;
using Ghost.Model.Messaging;
using Ghost.Xenus;
using System;
using System.Collections.ObjectModel;
using System.Net.WebSockets;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Input;
using Timer = System.Timers.Timer;

namespace Ghost.ViewModel.Controls
{
    public class ChatViewModel : ViewModelBase
    {
        private readonly Timer _killChatTimer;

        public string CurrentChatStatus { get; set; } = "idle";
        public int ActiveUserCount { get; set; }

        public Client Client { get; }
        public bool KillChatArmed { get; private set; }

        public bool WasMessageEmpty { get; set; } = true;
        public string MessageContent { get; set; }

        public bool IsCurrentlyChatting => Client.IsCurrentlyChatting;

        public ObservableCollection<ChatItem> ChatItems { get; } = new ObservableCollection<ChatItem>();

        public ChatViewModel()
        {
            Messenger.Default.Register<EndOrStartNewChatMessage>(this,
                async (msg) => await EndOrStartNewChat(msg.Location));
            Messenger.Default.Register<ActiveUserCountMessage>(this, (msg) => ActiveUserCount = msg.CurrentCount);
            Messenger.Default.Register<ChatStatusMessage>(this, (msg) => CurrentChatStatus = msg.Status);

            _killChatTimer = new Timer(850);
            _killChatTimer.Elapsed += KillChatTimer_Elapsed;

            PropertyChanged += ChatViewModel_PropertyChanged;

            Client = new Client(false);
            Client.ActiveUserCountChanged += Client_ActiveUserCountChanged;
            Client.ChatMessageReceived += Client_ChatMessageReceived;
            Client.TopicReceived += Client_TopicReceived;
            Client.ChatMessageSent += Client_ChatMessageSent;
            Client.TypingStateChanged += Client_TypingStateChanged;
            Client.ChatStarted += Client_ChatStarted;
            Client.ChatEnded += Client_ChatEnded;
            Client.RawPacketReceived += Client_RawPacketReceived;
            Client.RawPacketSent += Client_RawPacketSent;

            if (!IsInDesignMode)
#pragma warning disable CS4014
                Client.Connect();
#pragma warning restore CS4014
        }

        private async void ChatViewModel_PropertyChanged(object sender,
            System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(MessageContent))
            {
                if (WasMessageEmpty && MessageContent.Length > 0)
                {
                    await Client.SendTypingState(true);
                    WasMessageEmpty = false;
                }
                else if (!WasMessageEmpty && MessageContent.Length == 0)
                {
                    if (Client.IsCurrentlyChatting)
                        await Client.SendTypingState(false);

                    WasMessageEmpty = true;
                }
            }
        }

        [AsyncCommand]
        public async Task SendMessage()
        {
            if (!Client.IsConnectionOpen || !Client.IsCurrentlyChatting || string.IsNullOrEmpty(MessageContent))
                return;

            await Client.SendMessage(MessageContent);
            MessageContent = string.Empty;
        }

        [Command]
        public void MessageTextBoxEnabled(TextBox textBox)
        {
            if (textBox.IsEnabled)
            {
                Keyboard.Focus(textBox);
            }
        }

        private async Task EndOrStartNewChat(Location location)
        {
            if (IsCurrentlyChatting)
            {
                if (KillChatArmed)
                {
                    _killChatTimer.Stop();
                    KillChatArmed = false;

                    await Client.EndChat();
                }
                else
                {
                    KillChatArmed = true;
                    _killChatTimer.Start();
                }
            }
            else if (!Client.IsSearchingForChat)
            {
                Messenger.Default.Send(new ChatStatusMessage("looking for a chat..."));
                await Client.StartNewChat(location);
            }
        }

        private void KillChatTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            KillChatArmed = false;
            _killChatTimer.Stop();
        }

        private void Client_TypingStateChanged(object sender, TypingStateEventArgs e)
        {
            if (e.IsTyping)
            {
                Messenger.Default.Send(new ChatStatusMessage("stranger is typing"));
            }
            else
            {
                Messenger.Default.Send(new ChatStatusMessage("stranger is idle"));
            }
        }

        private void Client_ChatStarted(object sender, ChatEventArgs e)
        {
            RaisePropertyChanged(nameof(IsCurrentlyChatting));

            var location = (Location)int.Parse(e.ChatInfo.Key.Split(":")[0]);

            Application.Current.Dispatcher.Invoke(() =>
            {
                ChatItems.Add(new Notification($"chat started (stranger is probably looking for people in {location})"));
            });
            Messenger.Default.Send((new ChatStatusMessage("connected to stranger")));
        }
        
        private void Client_ChatEnded(object sender, ChatEventArgs e)
        {
            RaisePropertyChanged(nameof(IsCurrentlyChatting));

            MessageContent = string.Empty;

            Application.Current.Dispatcher.Invoke(() => { ChatItems.Add(new Notification("chat ended")); });
            Messenger.Default.Send(new ChatStatusMessage("idle"));
        }
        
        private void Client_TopicReceived(object sender, TopicEventArgs e)
        {
            Application.Current.Dispatcher.Invoke(() =>
                ChatItems.Add(new Notification($"conversation topic\n{e.Topic.Body}")));
        }

        private void Client_ChatMessageReceived(object sender, ChatMessageEventArgs e)
        {
            Application.Current.Dispatcher.Invoke(() => ChatItems.Add(new Message(e.Message.Body, false)));
            Messenger.Default.Send(new ChatStatusMessage("stranger is idle"));
        }

        private async void Client_ChatMessageSent(object sender, ChatMessageEventArgs e)
        {
            WasMessageEmpty = true;
            await Client.SendTypingState(false);
            Application.Current.Dispatcher.Invoke(() => ChatItems.Add(new Message(e.Message.Body, true)));
        }

        private void Client_ActiveUserCountChanged(object sender, UserCountEventArgs e)
            => Messenger.Default.Send(new ActiveUserCountMessage(e.Count));

        private void Client_RawPacketSent(object sender, RawDataEventArgs e)
            => Console.WriteLine($" <- {e.RawPacket}");

        private void Client_RawPacketReceived(object sender, RawDataEventArgs e)
            => Console.WriteLine($" -> {e.RawPacket}");
    }
}