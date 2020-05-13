using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections.Immutable;

namespace wpay.Library.Frameworks.PayQueue.Publish
{
    using MessageType = Type;
    public class PublisherFactory
    {
        private PublishCommandCatalog _commandCatalog;
        private PublishEventCatalog _eventCatalog;

        public PublisherFactory(PublishCommandCatalog commandCatalog, PublishEventCatalog eventCatalog)
        {
            _commandCatalog = commandCatalog;
            _eventCatalog = eventCatalog;
        }

        public Publisher New(IExchangePublisher publisher) => 
            new Publisher(publisher, _commandCatalog, _eventCatalog);
        
    }
}