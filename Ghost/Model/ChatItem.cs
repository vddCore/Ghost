using System;

namespace Ghost.Model
{
    public abstract class ChatItem
    {
        public DateTime CreatedAt { get; protected set; } = DateTime.Now;
        public string Body { get; protected set; }

        public ChatItem(string body)
            => Body = body;
    }
}
