using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text.Json;
using System.Text;


namespace wpay.Library.Frameworks.PayQueue
{
    using GetRoutePublishFunc = Func<Type, object, string>;
    using GetRouteSendFunc = Func<Type, Type, string>;
    public class Publisher
    {
        private  GetRoutePublishFunc _publish;
        private  GetRouteSendFunc _send;
        private IExchangePublisher _publisher;
        private Datagram _datagram;

        public Publisher(GetRoutePublishFunc publish, GetRouteSendFunc send, IExchangePublisher publisher, Datagram datagram) =>
            (_publish, _send, _publisher, _datagram) = (publish, send, publisher, datagram);

        public async Task Send<S, T>(T message) where S : IServiceDefinition, new() 
        {
            var route = _send(typeof(S), typeof(T));
            var binMessage = DoEncode<T>(message);
            await _publisher.PublishInput(route, binMessage);
        }

        public async Task Publish<T>(T message) 
        {
            var route = _publish(typeof(T), message);
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