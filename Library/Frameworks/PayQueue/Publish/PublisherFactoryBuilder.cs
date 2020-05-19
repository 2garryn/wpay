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
        public PublisherFactoryBuilder(Routes routes, DepsCatalog depsCatalog)
        {
            _eventCatalog = new PublishEventCatalogBuilder(routes, depsCatalog);
            _commandCatalog = new PublishCommandCatalogBuilder(routes, depsCatalog);
            _deps = depsCatalog;
        }

        public void Command<S, T>() where S : IServiceDefinition, new() => _commandCatalog.Command<S, T>();
        public void PublishEvent<T>() => _eventCatalog.PublishEvent<T>();
        public void PublishEvent<T>(Func<T, string> route) => _eventCatalog.PublishEvent<T>(route);

        public PublisherFactory Build()
        {
            return new PublisherFactory(_commandCatalog.Build(), _eventCatalog.Build(), _deps);
        }
    }
}