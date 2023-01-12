using Ghost.Xenus.Protocol.Events.Data;

namespace Ghost.Xenus
{
    public class TopicEventArgs
    {
        public Topic Topic { get; }

        internal TopicEventArgs(Topic topic)
            => Topic = topic;
    }
}
