using System;
using System.Collections.Generic;
using System.Collections.Immutable;


namespace wpay.Library.Frameworks.PayQueue.Consume
{
    public class ConsumeEventCatalogBuilder
    {
        private Dictionary<Type, ICallbackExecutor> _catalog;
        private readonly ConsumeEventRoute _consumeRoutes;
        private readonly ContextFactory _contextFactory;
        public ConsumeEventCatalogBuilder(Routes routes, ContextFactory contextFactory)
        {
            _catalog = new Dictionary<Type, ICallbackExecutor>();
            _consumeRoutes = new ConsumeEventRoute(routes);
            _contextFactory = contextFactory;
        }

        public void Add(Type t, IServiceDefinition servDef, ICallbackExecutor executor)
        {
            _catalog[t] = executor;
            _consumeRoutes.Add(t, servDef);
        }
        public void Add(Type t, IServiceDefinition servDef, string routeKey, ICallbackExecutor executor)
        {
            _catalog[t] = executor;
            _consumeRoutes.Add(t, servDef, routeKey);
        }

        public IConsumeExecutor GetExecuter() =>
            new ConsumeExecutorCatalog(_catalog.ToImmutableDictionary(), _contextFactory);

        public ConsumeEventRoute GetConsumeRoute() =>
            _consumeRoutes;
    }
}