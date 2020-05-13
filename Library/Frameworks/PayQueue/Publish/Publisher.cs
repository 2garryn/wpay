using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text.Json;
using System.Text;


namespace wpay.Library.Frameworks.PayQueue.Publish
{
    public class Publisher
    {
        private readonly PublishEventCatalog _eventCatalog;
        private readonly PublishCommandCatalog _commandCatalog;
        private readonly IExchangePublisher _publisher;

        public Publisher(IExchangePublisher publisher, PublishCommandCatalog commandCatalog,
            PublishEventCatalog eventCatalog)
        {
            _publisher = publisher;
            _eventCatalog = eventCatalog;
            _commandCatalog = commandCatalog;
        }

        public async Task Command<S, T>(T message) where S : IServiceDefinition, new() =>
            await Command<S, T>(message, (ICallParameters parameters) => 
            {
                parameters.ConversationId = null;
            });
        

        public async Task Command<S, T>(T message, Action<ICallParameters> parameters) where S : IServiceDefinition, new()
        {
            var route = _commandCatalog.GetRoute<S, T>();
            var binMessage = DoEncode<T>(message, parameters);
            await _publisher.Command(route, binMessage);
        }

        public async Task Publish<T>(T message) =>
            await Publish<T>(message, (ICallParameters parameters) => 
            {
                parameters.ConversationId = null;
            });
        

        public async Task Publish<T>(T message, Action<ICallParameters> parameters)
        {
            var route = _eventCatalog.GetRoute(message);
            var binMessage = DoEncode<T>(message, parameters);
            await _publisher.PublishEvent(route, binMessage);
        }

        private byte[] DoEncode<T>(T message, Action<ICallParameters> parameters)
        {
            var callParams = new CallParameters();
            parameters(callParams);
            var serMessage = JsonSerializer.Serialize(message);
            var datagram = new Datagram
            {
                OptionalParams = new DatagramOptionalParams
                {
                    ConversationId = callParams.ConversationId
                },
                Id = Guid.NewGuid(),
                Type = typeof(T).FullName,
                Message = Encoding.UTF8.GetBytes(serMessage)
            };
            return Encoding.UTF8.GetBytes(JsonSerializer.Serialize(datagram));
        }

        
    }
}