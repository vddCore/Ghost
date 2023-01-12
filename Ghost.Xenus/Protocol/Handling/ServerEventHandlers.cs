using Ghost.Xenus.Protocol.Events;
using Ghost.Xenus.Protocol.Events.Data;
using Ghost.Xenus.Protocol.Packets;

namespace Ghost.Xenus.Protocol.Handling
{
    internal class ServerEventHandlers
    {
        [ServerEventHandler("cn_acc")]
        internal static async void ConnectionAccepted(Event serverEvent, Client client)
        {
            var connectionInfo = serverEvent.DataAs<ConnectionInfo>();

            var cinfo = new ClientInfo { Hash = connectionInfo.Hash };

            var cinfoPacket = new EventPacket("_cinfo", cinfo);
            var owackPacket = new EventPacket("_owack", null);

            await client.SendEventPacket(cinfoPacket);
            await client.SendEventPacket(owackPacket);

            client.OnConnectionAccepted(connectionInfo);
        }

        [ServerEventHandler("talk_s")]
        internal static async void ChatStart(Event serverEvent, Client client)
        {
            var chatInfo = serverEvent.DataAs<ChatInfo>();
            var newConvAcknowledged = new ChatStartedAck { Key = chatInfo.Key };

            var begackedPacket = new EventPacket("_begacked", newConvAcknowledged, client.ClientEventID);
            await client.SendEventPacket(begackedPacket);

            client.OnChatStarted(chatInfo);
        }

        [ServerEventHandler("sdis")]
        internal static void ChatEnd(Event serverEvent, Client client)
        {
            var chatId = serverEvent.DataAs<ulong>();
            client.OnChatEnded(chatId);
        }

        [ServerEventHandler("count")]
        internal static void ActiveUserCount(Event serverEvent, Client client)
        {
            var count = serverEvent.DataAs<int>();
            client.OnActiveUserCountChanged(count);
        }

        [ServerEventHandler("styp")]
        internal static void TypingState(Event serverEvent, Client client)
        {
            var isTyping = serverEvent.DataAs<bool>();
            client.OnTypingStateChanged(isTyping);
        }

        [ServerEventHandler("rmsg")]
        internal static void Message(Event serverEvent, Client client)
        {
            var message = serverEvent.DataAs<ChatMessage>();
            client.OnChatMessageReceived(message);
        }

        [ServerEventHandler("r_svmsg")]
        internal static void ServiceMessage(Event serverEvent, Client client)
        {
            var message = serverEvent.DataAs<string>();
            client.OnServiceMessageReceived(message);
        }

        [ServerEventHandler("rtopic")]
        internal static void RandomTopic(Event serverEvent, Client client)
        {
            var topic = serverEvent.DataAs<Topic>();
            client.OnTopicReceived(topic);
        }
    }
}
