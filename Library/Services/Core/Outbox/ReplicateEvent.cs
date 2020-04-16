using System;
using System.Collections.Generic;
using System.Linq;

namespace wpay.Library.Services.Core.Outbox
{

    public class ReplicateEvent
    {
        public Guid Id {get;set;}
        public string EventType {get;set;}
        public string Event {get;set;}

    }


}