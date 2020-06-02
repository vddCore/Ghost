using System;

namespace Ghost.Xenus
{
    [AttributeUsage(AttributeTargets.Method)]
    public class ServerEventHandlerAttribute : Attribute
    {
        public string EventName { get; }

        public ServerEventHandlerAttribute(string eventName)
            => EventName = eventName;
    }
}
