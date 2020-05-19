using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using Microsoft.Extensions.Logging;

namespace wpay.Library.Frameworks.PayQueue.Publish
{
    internal class PublishEventCatalogBuilder
    {
        private  Dictionary<Type, Func<object, string>> _catalog;
        private Routes _routes;
        private DepsCatalog _deps;

        public PublishEventCatalogBuilder(Routes routes, DepsCatalog deps)
        {
            _routes = routes;
            _catalog = new Dictionary<Type, Func<object, string>>();
            _deps = deps;
        }

        public void PublishEvent<T>()
        {
            var path = _routes.PublishEventExchange<T>();
            _catalog[typeof(T)] = path;
            _deps.Logger.LogDebug($"Define publish event {typeof(T).FullName}, dynamic route");
        }

        public void PublishEvent<T>(Func<T, string> route)
        {
            _catalog[typeof(T)] = _routes.PublishEventExchange<T>(route);
            _deps.Logger.LogDebug($"Define publish event {typeof(T).FullName}, dynamic route");
        }
        
        public PublishEventCatalog Build() => 
            new PublishEventCatalog(_catalog.ToImmutableDictionary());
 
    }
}