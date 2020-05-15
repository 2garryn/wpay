using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace wpay.Library.Frameworks.PayQueue.Consume
{
    public class ConsumeCommandCatalogBuilder
    {
        private Dictionary<Type, ICallbackExecutor> _catalog;
        private readonly ContextFactory _contextFactory;
        private readonly Routes _routes;
        public ConsumeCommandCatalogBuilder(Routes routes, ContextFactory contextFactory)
        {
            _catalog = new Dictionary<Type, ICallbackExecutor>();
            _routes = routes;
            _contextFactory = contextFactory;
        }

        public void Consume<T>(ICallbackExecutor executor) =>
            _catalog[typeof(T)] = executor;

        public IConsumeExecutor GetExecuter() =>
            new ConsumeExecutorCatalog(_catalog.ToImmutableDictionary(), _contextFactory);

        public ConsumeCommandRoute GetConsumeRoute() => new ConsumeCommandRoute(_routes, _catalog.Count > 0);

    }
}