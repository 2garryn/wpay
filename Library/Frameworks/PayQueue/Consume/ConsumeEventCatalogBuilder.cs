using System;
using System.Collections.Generic;
using System.Collections.Immutable;


namespace wpay.Library.Frameworks.PayQueue.Consume
{
    internal class ConsumeEventCatalogBuilder
    {
        private Dictionary<string, ICallbackExecutor> _catalog;
        private readonly ConsumeEventRoute _consumeRoutes;
        private readonly MessageContextFactory _contextFactory;
        public ConsumeEventCatalogBuilder(Routes routes, MessageContextFactory contextFactory)
        {
            _catalog = new Dictionary<string, ICallbackExecutor>();
            _consumeRoutes = new ConsumeEventRoute(routes);
            _contextFactory = contextFactory;
        }

        public void Consume<S, T>(ICallbackExecutor executor) 
            where S : IServiceDefinition, new()
        {
            _catalog[typeof(T).FullName] = executor;
            _consumeRoutes.Add<S, T>();
        }
        public void Consume<S, T>(string routeKey, ICallbackExecutor executor) 
            where S : IServiceDefinition, new()
        {
            _catalog[typeof(T).FullName] = executor;
            _consumeRoutes.Add<S, T>(routeKey);
        }

        public IConsumeExecutor GetExecuter() =>
            new ConsumeExecutorCatalog(_catalog.ToImmutableDictionary(), _contextFactory);

        public ConsumeEventRoute GetConsumeRoute() =>
            _consumeRoutes;
    }
}