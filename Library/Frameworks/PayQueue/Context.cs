using System;
using System.Collections.Generic;
using System.Linq;

namespace wpay.Library.Frameworks.PayQueue
{

    public class Context
    {
        public Guid ConversationId {get;}
        public IPublisher Publisher {get;}
        internal Context(Guid converstaionId, IPublisher publisher) =>
            (ConversationId, Publisher) = (converstaionId, publisher);

    }


}