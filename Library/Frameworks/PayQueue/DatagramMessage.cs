using System;
using System.Collections.Generic;
using System.Linq;

namespace wpay.Library.Frameworks.PayQueue
{

    public class DatagramMessage<T>
    {
        public Guid RequestId {get;set;}
        public T Message {get;set;}
        public string SourceService {get;set;}
        public string SourceHost {get;set;}
        public string DestinationService {get;set;}
        public DateTime PublishTimestamp {get;set;}
        public Guid? ConversationId {get;set;}
    }


}