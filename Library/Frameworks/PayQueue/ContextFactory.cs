using System;
using System.Text.Json;
using wpay.Library.Frameworks.PayQueue.Publish;

namespace wpay.Library.Frameworks.PayQueue
{

    public class ContextFactory
    {
        private PublisherFactory _publisherFactory;

        public ContextFactory(PublisherFactory publisherFactory)
        {
            _publisherFactory = publisherFactory;
        }
        
        public (Context, Type, byte[]) New(IExchangePublisher exchangePublisher, byte[] data)
        {
            var datagram = ParseDatagram(data);
            var t = Type.GetType(datagram.Type);
            var context = new Context(
                datagram.Id,
                datagram.OptionalParams.ConversationId,
                _publisherFactory.New(exchangePublisher)
            );
            return (context, t, datagram.Message);
        }

        private Datagram ParseDatagram(byte[] data)
        {
            try
            {
                return JsonSerializer.Deserialize<Datagram>(data);
            }
            catch (JsonException e)
            {
                var asStirng = System.Text.Encoding.UTF8.GetString(data);
                throw new PayQueueException($"Can not deserialize Datagram. Data: {asStirng}", e);
            }
        }
    }
}