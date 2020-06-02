using Ghost.Xenus;

namespace Ghost.Model
{
    public class Notification : ChatItem
    {
        public ChatState State { get; }

        public Notification(ChatState state, string body)
            : base(body)
        {
            State = state;
        }
    }
}
