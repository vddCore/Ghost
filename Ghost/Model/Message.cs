namespace Ghost.Model
{
    public class Message : ChatItem
    {
        public bool IsOutgoing { get; }

        public Message(string body, bool isOutgoing)
            : base(body)
        {
            Body = body;
            IsOutgoing = isOutgoing;
        }
    }
}
