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
        private readonly string _sourceService;
        private readonly string _sourceHost;
        public PublisherFactory(PublishCommandCatalog commandCatalog, PublishEventCatalog eventCatalog, DepsCatalog deps, string sourceService, string sourceHost)
        {
            _commandCatalog = commandCatalog;
            _eventCatalog = eventCatalog;
            _deps = deps;
            _sourceService = sourceService;
            _sourceHost = sourceHost;
        }

        public Publisher New(IExchangePublisher publisher) => 
            new Publisher(publisher, _commandCatalog, _eventCatalog, _deps, _sourceService, _sourceHost);
        
    }
}