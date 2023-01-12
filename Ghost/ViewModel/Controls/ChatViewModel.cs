using Atlas.UI.Controls;
using DevExpress.Mvvm;
using DevExpress.Mvvm.DataAnnotations;
using Ghost.Model;
using Ghost.Model.Messaging;
using Ghost.Xenus;
using System;
using System.Collections.ObjectModel;
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

        public Client Client { get; }
        public bool KillChatArmed { get; private set; }

        public string EndChatButtonHint
        {
            get
            {
                if (KillChatArmed)
                    return "You sure?\n(Esc)";

                if (IsCurrentlyChatting)
                    return "End chat\n(Esc)";

                return "New chat\n(Esc)";
            }
        }

        public string MessageContent { get; set; }

        public bool IsCurrentlyChatting => Client.IsCurrentlyChatting;

        public ObservableCollection<ChatItem> ChatItems { get; } = new ObservableCollection<ChatItem>();

        public ChatViewModel()
        {
            RegisterMessageHandlers();

            _killChatTimer = new Timer(850);
            _killChatTimer.Elapsed += KillChatTimer_Elapsed;

            Client = new Client();
            Client.ChatMessageReceived += Client_ChatMessageReceived;
            Client.ChatMessageSent += Client_ChatMessageSent;
            Client.ChatStarted += Client_ChatStarted;
            Client.ChatEnded += Client_ChatEnded;
            Client.RawPacketReceived += Client_RawPacketReceived;
            Client.RawPacketSent += Client_RawPacketSent;

            if (!IsInDesignMode)
#pragma warning disable CS4014
                Client.Connect();
#pragma warning restore CS4014
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

        private async Task EndOrStartNewChat()
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
            else
            {
                await Client.StartNewChat();
            }

            RaisePropertyChanged(nameof(EndChatButtonHint));
        }

        private void RegisterMessageHandlers()
        {
            Messenger.Default.Register<EndOrStartNewChatMessage>(this, async (msg) =>
            {
                await EndOrStartNewChat();
            });
        }

        private void KillChatTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            KillChatArmed = false;
            _killChatTimer.Stop();
        }

        private void Client_ChatStarted(object sender, ChatEventArgs e)
        {
            RaisePropertyChanged(nameof(IsCurrentlyChatting));
            RaisePropertyChanged(nameof(EndChatButtonHint));

            Application.Current.Dispatcher.Invoke(() =>
            {
                ChatItems.Add(new Notification("chat started"));
            });
        }

        private void Client_ChatEnded(object sender, ChatEventArgs e)
        {
            RaisePropertyChanged(nameof(IsCurrentlyChatting));
            RaisePropertyChanged(nameof(EndChatButtonHint));

            MessageContent = string.Empty;

            Application.Current.Dispatcher.Invoke(() =>
            {
                ChatItems.Add(new Notification("chat ended"));
            });
        }

        private void Client_RawPacketSent(object sender, RawDataEventArgs e)
            => Console.WriteLine($" <- {e.RawPacket}");

        private void Client_RawPacketReceived(object sender, RawDataEventArgs e)
            => Console.WriteLine($" -> {e.RawPacket}");

        private void Client_ChatMessageReceived(object sender, ChatMessageEventArgs e)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                ChatItems.Add(new Message(e.Message.Body, false));
            });
        }

        private void Client_ChatMessageSent(object sender, ChatMessageEventArgs e)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                ChatItems.Add(new Message(e.Message.Body, true));
            });
        }
    }
}
