using Ghost.Xenus;

namespace Ghost.Model.Messaging
{
    public class EndOrStartNewChatMessage
    {
        public Location Location { get; set; }
        public bool Force { get; set; }
    }
}
