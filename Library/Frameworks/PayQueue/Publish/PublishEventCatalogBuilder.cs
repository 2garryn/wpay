using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace wpay.Library.Frameworks.PayQueue.Publish
{
    public class PublishEventCatalogBuilder
    {
        private  Dictionary<Type, Func<object, string>> _catalog;
        private Routes _routes;

        public PublishEventCatalogBuilder(Routes routes)
        {
            _routes = routes;
        }
        public void PublishEvent<T>() => _catalog[typeof(T)] = _routes.PublishEventExchange<T>(null);
        public void PublishEvent<T>(Func<T, string> route) => _catalog[typeof(T)] = _routes.PublishEventExchange<T>(route);
        
        public PublishEventCatalog Build() => 
            new PublishEventCatalog(_catalog.ToImmutableDictionary());
 
    }
}