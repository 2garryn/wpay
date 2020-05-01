using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections.Immutable;

namespace wpay.Library.Frameworks.PayQueue
{
    using MessageType = Type;
    public class PublisherFactory
    {
        internal ImmutableDictionary<Type, ImmutableDictionary<MessageType, string>> InputSendRoutes {get;set;}
        internal ImmutableDictionary<MessageType, Func<object, string>> EventRoutes {get;set;}

        public Publisher ToPublisher(IExchangePublisher publisher, Datagram datagram)
        {
            return new Publisher(
                (t, obj) => EventRoutes[t](obj),
                (st, tt) => InputSendRoutes[st][tt],
                publisher,
                datagram
            );
        }

        

    }


}