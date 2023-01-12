using DevExpress.Mvvm;
using DevExpress.Mvvm.DataAnnotations;
using Ghost.Model;
using Ghost.Model.Messaging;
using Ghost.Xenus;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows;

namespace Ghost.ViewModel.Controls
{
    public class ChatViewModel : ViewModelBase
    {
        public Client Client { get; }

        public string MessageContent { get; set; }
        public bool IsCurrentlyChatting { get; private set; }

        public ObservableCollection<ChatItem> Messages { get; set; }

        public ChatViewModel()
        {
            Messenger.Default.Register<StartNewChatMessage>(this, StartNewChat);
            Messages = new ObservableCollection<ChatItem>();

            Client = new Client();
            Client.ChatMessageReceived += Client_ChatMessageReceived;
            Client.ChatMessageSent += Client_ChatMessageSent;
            Client.ChatStarted += Client_ChatStarted;
            Client.ChatEnded += Client_ChatEnded;

            Client.RawPacketReceived += Client_RawPacketReceived;
            Client.RawPacketSent += Client_RawPacketSent;

            Client.Connect();
        }

        private void Client_ChatStarted(object sender, ChatEventArgs e)
        {
            IsCurrentlyChatting = Client.IsCurrentlyChatting;
        }

        private void Client_ChatEnded(object sender, ChatEventArgs e)
        {
            IsCurrentlyChatting = Client.IsCurrentlyChatting;
        }

        [AsyncCommand]
        public async Task SendMessage()
        {
            if (!Client.IsConnectionOpen || !Client.IsCurrentlyChatting || string.IsNullOrEmpty(MessageContent))
                return;

            await Client.SendMessage(MessageContent);
            MessageContent = string.Empty;
        }

        private async void StartNewChat(StartNewChatMessage msg)
            => await Client.StartNewChat();

        private void Client_RawPacketSent(object sender, RawDataEventArgs e)
            => Console.WriteLine($" <- {e.RawPacket}");

        private void Client_RawPacketReceived(object sender, RawDataEventArgs e)
            => Console.WriteLine($" -> {e.RawPacket}");

        private void Client_ChatMessageReceived(object sender, ChatMessageEventArgs e)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                Messages.Add(new Message(e.Message.Body, false));
            });
        }

        private void Client_ChatMessageSent(object sender, ChatMessageEventArgs e)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                Messages.Add(new Message(e.Message.Body, true));
            });
        }
    }
}
