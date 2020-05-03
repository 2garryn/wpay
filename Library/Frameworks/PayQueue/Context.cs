using System;
using System.Collections.Generic;
using System.Linq;

namespace wpay.Library.Frameworks.PayQueue
{

    public class Context
    {
        public Guid Id { get; }
        public Guid? ConversationId { get; }
        public Publisher Publisher { get; }
        internal Context(Guid id, Guid? converstaionId, Publisher publisher) =>
            (Id, ConversationId, Publisher) = (id, converstaionId, publisher);
    }

}