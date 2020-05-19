using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections.Immutable;

namespace wpay.Library.Frameworks.PayQueue.Publish
{
    using MessageType = Type;
    internal class PublisherFactory
    {
        private PublishCommandCatalog _commandCatalog;
        private PublishEventCatalog _eventCatalog;
        private DepsCatalog _deps;
        public PublisherFactory(PublishCommandCatalog commandCatalog, PublishEventCatalog eventCatalog, DepsCatalog deps)
        {
            _commandCatalog = commandCatalog;
            _eventCatalog = eventCatalog;
            _deps = deps;
        }

        public Publisher New(IExchangePublisher publisher) => 
            new Publisher(publisher, _commandCatalog, _eventCatalog, _deps);
        
    }
}