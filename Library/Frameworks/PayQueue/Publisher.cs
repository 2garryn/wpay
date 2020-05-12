using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text.Json;
using System.Text;


namespace wpay.Library.Frameworks.PayQueue
{
    using GetRoutePublishFunc = Func<Type, object, string>;
    using GetRouteCommandFunc = Func<Type, Type, string>;

    public class Publisher
    {
        private readonly GetRoutePublishFunc _publishRoute;
        private readonly GetRouteCommandFunc _commandRoute;
        private readonly IExchangePublisher _publisher;

        public Publisher(GetRoutePublishFunc publishRoute, GetRouteCommandFunc commandRoute, IExchangePublisher publisher) =>
            (_publishRoute, _commandRoute, _publisher) = (publishRoute, commandRoute, publisher);

        public async Task Command<S, T>(T message) where S : IServiceDefinition, new() =>
            await Command<S, T>(message, (ICallParameters parameters) => 
            {
                parameters.ConversationId = null;
            });
        

        public async Task Command<S, T>(T message, Action<ICallParameters> parameters) where S : IServiceDefinition, new()
        {
            var route = _commandRoute(typeof(S), typeof(T));
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
            var route = _publishRoute(typeof(T), message);
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