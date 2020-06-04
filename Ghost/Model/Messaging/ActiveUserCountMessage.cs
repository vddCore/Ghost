namespace Ghost.Model.Messaging
{
    internal class ActiveUserCountMessage
    {
        public int CurrentCount { get; }

        public ActiveUserCountMessage(int currentCount)
            => CurrentCount = currentCount;
    }
}
