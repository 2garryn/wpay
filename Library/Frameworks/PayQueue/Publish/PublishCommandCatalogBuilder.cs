using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.Extensions.Logging;

namespace wpay.Library.Frameworks.PayQueue.Publish
{
    using MessageType = Type;
    using ServerType = Type;
    internal class PublishCommandCatalogBuilder
    {
        private Dictionary<MessageType, Dictionary<MessageType, string>> _catalog;
        private Routes _routes;
        private DepsCatalog _deps;

        public PublishCommandCatalogBuilder(Routes routes, DepsCatalog deps)
        {
            _catalog = new Dictionary<Type, Dictionary<Type, string>>();
            _routes = routes;
            _deps = deps;
        }
        
        
        public void Command<S, T>() where S : IServiceDefinition, new()
        {
            var path = _routes.PublishCommandExchange(new S().Label());
            var servtype = typeof(S);
            var msgType = typeof(T);
            Dictionary<MessageType, string> messages;
            if (_catalog.TryGetValue(servtype, out messages))
            {
                messages.Add(msgType, path);
            }
            else
            {
                messages = new Dictionary<MessageType, string>() { [msgType] = path };
                _catalog.Add(servtype, messages);
            }
            _deps.Logger.LogDebug($"Define publish command {typeof(S).FullName}:{typeof(T).FullName}, route \"{path}\"");

        }

        public PublishCommandCatalog Build()
        {
            var immutableCatalog = _catalog
                .ToDictionary(k => k.Key, v => v.Value.ToImmutableDictionary())
                .ToImmutableDictionary();
            return new PublishCommandCatalog(immutableCatalog);
        }
        
    }
}