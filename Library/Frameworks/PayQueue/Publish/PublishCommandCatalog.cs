using System;
using System.Collections.Immutable;
using System.Threading.Tasks;

namespace wpay.Library.Frameworks.PayQueue.Publish
{
    public class PublishCommandCatalog
    {
        private  ImmutableDictionary<Type, ImmutableDictionary<Type, string>> _catalog;

        public PublishCommandCatalog(ImmutableDictionary<Type, ImmutableDictionary<Type, string>> catalog)
        {
            _catalog = catalog;
        }
        public string GetRoute<S, T>() => _catalog[typeof(S)][typeof(T)];
    }
}