using System;

namespace wpay.Library.Frameworks.PayQueue.Publish
{
    public class PublisherFactoryBuilder
    {
        private PublishCommandCatalogBuilder _commandCatalog;
        private PublishEventCatalogBuilder _eventCatalog;
        public PublisherFactoryBuilder(Routes routes)
        {
            _eventCatalog = new PublishEventCatalogBuilder(routes);
            _commandCatalog = new PublishCommandCatalogBuilder(routes);
        }

        public void Command<S, T>() where S : IServiceDefinition, new() => _commandCatalog.Command<S, T>();
        public void PublishEvent<T>() => _eventCatalog.PublishEvent<T>();
        public void PublishEvent<T>(Func<T, string> route) => _eventCatalog.PublishEvent<T>(route);

        public PublisherFactory Build()
        {
            return new PublisherFactory(_commandCatalog.Build(), _eventCatalog.Build());
        }
    }
}