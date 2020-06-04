namespace Ghost.Model.Messaging
{
    internal class ChatStatusMessage
    {
        public string Status { get; set; }

        public ChatStatusMessage(string status)
            => Status = status;
    }
}