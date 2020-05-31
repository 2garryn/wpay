using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading.Tasks;
using GreenPipes;

namespace wpay.Library.Frameworks.PayQueue.Publish
{
    public class PublishCommandCatalog
    {
        private  ImmutableDictionary<Type, ImmutableDictionary<Type, string>> _catalog;

        public PublishCommandCatalog(ImmutableDictionary<Type, ImmutableDictionary<Type, string>> catalog)
        {
            _catalog = catalog;
        }

        public string GetRoute<S, T>()
        {
            try
            {
                return _catalog[typeof(S)][typeof(T)];
            }
            catch (KeyNotFoundException)
            {
                var excp = new PayloadException("Can not find publish command");
                excp.Data["Service"] = typeof(S).FullName;
                excp.Data["Command"] = typeof(T).FullName;
                throw excp;
            }
        }
    }
}