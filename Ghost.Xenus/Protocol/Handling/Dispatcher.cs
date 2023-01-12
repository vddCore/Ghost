using Ghost.Xenus.Protocol.Events;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Ghost.Xenus.Protocol.Handling
{
    public class Dispatcher
    {
        private Client _client;
        private Dictionary<string, List<ServerResponseProcessor>> _processors;

        public delegate void ServerResponseProcessor(Event ev, Client client);

        public Dispatcher(Client client)
        {
            _client = client;
            _processors = new Dictionary<string, List<ServerResponseProcessor>>();
        }

        public void Dispatch(Event serverEvent)
        {
            if (_processors.ContainsKey(serverEvent.Name))
            {
                foreach (var processor in _processors[serverEvent.Name])
                    processor(serverEvent, _client);
            }
            else
            {
                Console.WriteLine($"Dispatcher: Unknown server event '{serverEvent.Name}'");
            }
        }

        public void RegisterResponseProcessor(string eventName, ServerResponseProcessor processor)
        {
            if (!_processors.ContainsKey(eventName))
                _processors.Add(eventName, new List<ServerResponseProcessor>());

            _processors[eventName].Add(processor);
        }

        public void AddProcessorsFrom(Assembly assembly)
        {
            var types = assembly.GetTypes();
            foreach (var type in types)
            {
                var methods = type.GetRuntimeMethods();
                foreach (var method in methods)
                {
                    if (method.IsStatic)
                    {
                        var attr = method.GetCustomAttribute<ServerEventHandlerAttribute>();
                        
                        if (attr != null)
                            RegisterResponseProcessor(attr.EventName, (ServerResponseProcessor)method.CreateDelegate(typeof(ServerResponseProcessor)));
                    }
                }
            }
        }
    }
}
