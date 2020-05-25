using System;
using System.Collections.Generic;
using System.Linq;
using wpay.Library.Frameworks.PayQueue.Publish;

namespace wpay.Library.Frameworks.PayQueue
{

    public class MessageContext<T>
    {
        public T Message { get; internal set; }
        public Guid RequestId { get; internal set; }
        public string SourceService { get; internal set; }
        public string SourceHost { get; internal set; }
        public DateTime PublishTimestamp { get; internal set; }
        public Guid? ConversationId { get; internal set; }
        public Publisher Publisher {get; internal set;}
    }


}