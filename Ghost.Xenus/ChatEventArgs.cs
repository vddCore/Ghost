using Ghost.Xenus.Protocol.Events.Data;

namespace Ghost.Xenus
{
    public class ChatEventArgs
    {
        public ChatState State { get; }
        public ChatInfo ChatInfo { get; }

        internal ChatEventArgs(ChatState state, ChatInfo chatInfo)
        {
            State = state; 
            ChatInfo = chatInfo;
        }
    }
}
