namespace Ghost.Xenus
{
    public class ServiceMessageEventArgs
    {
        public string Body { get; }

        internal ServiceMessageEventArgs(string body)
            => Body = body;
    }
}
