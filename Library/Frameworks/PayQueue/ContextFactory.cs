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
            var datagram = JsonSerializer.Deserialize<Datagram>(data);
            var t = Type.GetType(datagram.Type);
            var context = new Context(
                datagram.Id,
                datagram.OptionalParams.ConversationId,
                _publisherFactory.New(exchangePublisher)
            );
            return (context, t, datagram.Message);
        }
    }
}