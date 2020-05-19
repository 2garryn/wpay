using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text.Json;
using System.Text;
using Microsoft.Extensions.Logging;


namespace wpay.Library.Frameworks.PayQueue.Publish
{
    public class Publisher
    {
        private readonly PublishEventCatalog _eventCatalog;
        private readonly PublishCommandCatalog _commandCatalog;
        private readonly IExchangePublisher _publisher;
        private readonly DepsCatalog _deps;

        internal Publisher(IExchangePublisher publisher, PublishCommandCatalog commandCatalog,
            PublishEventCatalog eventCatalog, DepsCatalog deps)
        {
            _publisher = publisher;
            _eventCatalog = eventCatalog;
            _commandCatalog = commandCatalog;
            _deps = deps;
        }

        public async Task Command<S, T>(T message) where S : IServiceDefinition, new() =>
            await Command<S, T>(message, (ICallParameters parameters) => 
            {
                parameters.ConversationId = null;
            });
        

        public async Task Command<S, T>(T message, Action<ICallParameters> parameters) where S : IServiceDefinition, new()
        {
            var route = _commandCatalog.GetRoute<S, T>();
            var (binMessage, id) = DoEncode<T>(message, parameters);
            _deps.Logger.LogDebug($"Publish command {typeof(S).Name}:{typeof(T).Name} to {route}. ID {id}");
            await _publisher.Command(route, binMessage);
            _deps.Logger.LogDebug($"Published command {typeof(S).Name}:{typeof(T).Name} to {route} successfully. ID {id}");
        }

        public async Task Publish<T>(T message) =>
            await Publish<T>(message, (ICallParameters parameters) => 
            {
                parameters.ConversationId = null;
            });
        

        public async Task Publish<T>(T message, Action<ICallParameters> parameters)
        {
            var route = _eventCatalog.GetRoute(message);
            var (binMessage, id) = DoEncode<T>(message, parameters);
            _deps.Logger.LogDebug($"Publish event {typeof(T).Name} to {route}. ID: {id}");
            await _publisher.PublishEvent(route, binMessage);
            _deps.Logger.LogDebug($"Published event {typeof(T).Name} to {route} successfully. ID: {id}");
        }

        private (byte[], Guid) DoEncode<T>(T message, Action<ICallParameters> parameters)
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
            return (Encoding.UTF8.GetBytes(JsonSerializer.Serialize(datagram)), datagram.Id);
        }

        
    }
}