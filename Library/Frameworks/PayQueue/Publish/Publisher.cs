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
        private readonly string _sourceService;
        private readonly string _sourceHost;

        internal Publisher(IExchangePublisher publisher, PublishCommandCatalog commandCatalog,
            PublishEventCatalog eventCatalog, DepsCatalog deps, string sourceService, string sourceHost)
        {
            _publisher = publisher;
            _eventCatalog = eventCatalog;
            _commandCatalog = commandCatalog;
            _deps = deps;
            _sourceHost = sourceHost;
            _sourceService = sourceService;
        }

        public async Task Command<S, T>(T message) where S : IServiceDefinition, new() =>
            await Command<S, T>(message, (ICallParameters parameters) => 
            {
                parameters.ConversationId = null;
            });
        

        public async Task Command<S, T>(T message, Action<ICallParameters> parameters) where S : IServiceDefinition, new()
        {
            var route = _commandCatalog.GetRoute<S, T>();
            var datagram = NewDatagram<T>(message, parameters);
            var binMessage = Serialize(datagram);
            _deps.Logger.LogDebug($"Publish command {typeof(S).Name}:{typeof(T).Name} to {route}. ID {datagram.RequestId}");
            await _publisher.Command(route, typeof(T).FullName, binMessage);
            _deps.Logger.LogDebug($"Published command {typeof(S).Name}:{typeof(T).Name} to {route} successfully. ID {datagram.RequestId}");
        }

        public async Task Publish<T>(T message) =>
            await Publish<T>(message, (ICallParameters parameters) => 
            {
                parameters.ConversationId = null;
            });
        

        public async Task Publish<T>(T message, Action<ICallParameters> parameters)
        {
            var route = _eventCatalog.GetRoute(message);
            var datagram = NewDatagram<T>(message, parameters);
            var binMessage = Serialize(datagram);
            _deps.Logger.LogDebug($"Publish event {typeof(T).Name} to {route}. ID: {datagram.RequestId}");
            await _publisher.PublishEvent(route, typeof(T).FullName, binMessage);
            _deps.Logger.LogDebug($"Published event {typeof(T).Name} to {route} successfully. ID: {datagram.RequestId}");
        }
        

        private DatagramMessage<T> NewDatagram<T>(T message, Action<ICallParameters> p)
        {
            var callParameters = new CallParameters();
            p?.Invoke(callParameters);
            return new DatagramMessage<T>()
            {
                RequestId = callParameters.RequestId ?? Guid.NewGuid(),
                Message = message,
                ConversationId = callParameters.ConversationId,
                PublishTimestamp = DateTime.UtcNow,
                SourceHost = _sourceHost,
                SourceService = _sourceService
            };
        }

        private byte[] Serialize<T>(T value) => Encoding.UTF8.GetBytes(JsonSerializer.Serialize(value));


    }
}