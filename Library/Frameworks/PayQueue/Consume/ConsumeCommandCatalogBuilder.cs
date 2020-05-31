using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace wpay.Library.Frameworks.PayQueue.Consume
{
    internal class ConsumeCommandCatalogBuilder
    {
        private Dictionary<string, ICallbackExecutor> _catalog;
        private readonly MessageContextFactory _contextFactory;
        private readonly Routes _routes;
        private readonly DepsCatalog _deps;
        public ConsumeCommandCatalogBuilder(Routes routes, MessageContextFactory contextFactory, DepsCatalog deps)
        {
            _catalog = new Dictionary<string, ICallbackExecutor>();
            _routes = routes;
            _contextFactory = contextFactory;
            _deps = deps;
        }

        public void Consume<T>(ICallbackExecutor executor) =>
            _catalog[typeof(T).FullName] = executor;

        public IConsumeExecutor GetExecuter() =>
            new ConsumeExecutorCatalog(_catalog.ToImmutableDictionary(), _contextFactory, _deps);

        public ConsumeCommandRoute GetConsumeRoute() => new ConsumeCommandRoute(_routes, _catalog.Count > 0);

    }
}