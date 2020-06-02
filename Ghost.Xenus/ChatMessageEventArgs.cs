using Ghost.Xenus.Protocol.Events.Data;

namespace Ghost.Xenus
{
    public class ChatMessageEventArgs
    {
        public ChatMessage Message { get; }

        internal ChatMessageEventArgs(ChatMessage message)
            => Message = message;
    }
}
