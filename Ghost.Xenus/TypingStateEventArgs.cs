namespace Ghost.Xenus
{
    public class TypingStateEventArgs
    {
        public bool IsTyping { get; }

        internal TypingStateEventArgs(bool isTyping)
            => IsTyping = isTyping;
    }
}
