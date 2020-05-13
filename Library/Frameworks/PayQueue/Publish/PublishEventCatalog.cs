using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace wpay.Library.Frameworks.PayQueue.Publish
{
    public class PublishEventCatalog
    {
        private  ImmutableDictionary<Type, Func<object, string>> _catalog;

        public PublishEventCatalog(ImmutableDictionary<Type, Func<object, string>> catalog)
        {
            _catalog = catalog;
        }

        public string GetRoute<T>(T message) => _catalog[typeof(T)](message);

    }
}