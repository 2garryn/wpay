using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using wpay.Library.Frameworks.PayQueue.Publish;

namespace wpay.Library.Frameworks.PayQueue
{

    public class MessageContextFactory
    {
        private PublisherFactory _publisherFactory;
        internal MessageContextFactory(PublisherFactory publisherFactory)
        {
            _publisherFactory = publisherFactory;
        }

        public MessageContext<T> New<T>(IExchangePublisher _expPubl, byte[] data)
        {
            var datagram = JsonSerializer.Deserialize<DatagramMessage<T>>(data);
            return new MessageContext<T>()
            {
                RequestId = datagram.RequestId,
                Message = datagram.Message,
                SourceHost = datagram.SourceHost,
                SourceService = datagram.SourceService,
                PublishTimestamp = datagram.PublishTimestamp,
                ConversationId = datagram.ConversationId,
                Publisher = _publisherFactory.New(_expPubl)
            };
        }

    }


}