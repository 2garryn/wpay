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
        private readonly Datagram _datagram;

        public Publisher(GetRoutePublishFunc publishRoute, GetRouteCommandFunc commandRoute, IExchangePublisher publisher, Datagram datagram) =>
            (_publishRoute, _commandRoute, _publisher, _datagram) = (publishRoute, commandRoute, publisher, datagram);

        public async Task Command<S, T>(T message) where S : IServiceDefinition, new()
        {
            var route = _commandRoute(typeof(S), typeof(T));
            var binMessage = DoEncode<T>(message);
            await _publisher.Command(route, binMessage);
        }

        public async Task Publish<T>(T message)
        {
            var route = _publishRoute(typeof(T), message);
            var binMessage = DoEncode<T>(message);
            await _publisher.PublishEvent(route, binMessage);
        }

        private byte[] DoEncode<T>(T message)
        {
            var serMessage = JsonSerializer.Serialize(message);
            var datagram = new Datagram
            {
                ConversationId = _datagram.ConversationId,
                Type = typeof(T).FullName,
                Message = Encoding.UTF8.GetBytes(serMessage)
            };
            return Encoding.UTF8.GetBytes(JsonSerializer.Serialize(datagram));
        }

    }
}