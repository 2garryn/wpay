using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace wpay.Library.Frameworks.PayQueue.Consume
{
    public class ConsumeCommandCatalogBuilder
    {
        private Dictionary<Type, ICallbackExecutor> _catalog;
        private readonly ConsumeCommandRoute _consumeRoutes;
        private readonly ContextFactory _contextFactory;
        public ConsumeCommandCatalogBuilder(Routes routes, ContextFactory contextFactory)
        {
            _catalog = new Dictionary<Type, ICallbackExecutor>();
            _consumeRoutes = new ConsumeCommandRoute(routes);
            _contextFactory = contextFactory;
        }

        public void Add(Type t, ICallbackExecutor executor) =>
            _catalog[t] = executor;

        public IConsumeExecutor GetExecuter() =>
            new ConsumeExecutorCatalog(_catalog.ToImmutableDictionary(), _contextFactory);

        public ConsumeCommandRoute GetConsumeRoute() => _consumeRoutes;

    }
}