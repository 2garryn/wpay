using System;
using Microsoft.Extensions.Logging;
using wpay.Library.Frameworks.PayQueue;

namespace wpay.Library.Frameworks.PayQueue.Publish
{
    internal class PublisherFactoryBuilder
    {
        private PublishCommandCatalogBuilder _commandCatalog;
        private PublishEventCatalogBuilder _eventCatalog;
        private DepsCatalog _deps;
        private readonly string _sourceService;
        private readonly string _sourceHost;
        public PublisherFactoryBuilder(Routes routes, DepsCatalog depsCatalog, string sourceService, string sourceHost)
        {
            _eventCatalog = new PublishEventCatalogBuilder(routes, depsCatalog);
            _commandCatalog = new PublishCommandCatalogBuilder(routes, depsCatalog);
            _deps = depsCatalog;
            _sourceService = sourceService;
            _sourceHost = sourceHost;
        }

        public void Command<S, T>() where S : IServiceDefinition, new() => _commandCatalog.Command<S, T>();
        public void PublishEvent<T>() => _eventCatalog.PublishEvent<T>();
        public void PublishEvent<T>(Func<T, string> route) => _eventCatalog.PublishEvent<T>(route);

        public PublisherFactory Build()
        {
            return new PublisherFactory(_commandCatalog.Build(), _eventCatalog.Build(), _deps, _sourceService, _sourceHost);
        }
    }
}