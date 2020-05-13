using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace wpay.Library.Frameworks.PayQueue.Publish
{
    using MessageType = Type;
    using ServerType = Type;
    public class PublishCommandCatalogBuilder
    {
        private Dictionary<MessageType, Dictionary<MessageType, string>> _catalog;
        private Routes _routes;

        public PublishCommandCatalogBuilder(Routes routes)
        {
            _catalog = new Dictionary<Type, Dictionary<Type, string>>();
            _routes = routes;
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