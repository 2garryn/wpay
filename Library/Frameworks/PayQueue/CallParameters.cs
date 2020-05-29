using System;
using System.Collections.Generic;
using System.Linq;

namespace wpay.Library.Frameworks.PayQueue
{

    public class CallParameters: ICallParameters
    {
        public Guid? ConversationId {set; get;}
        public Guid? RequestId { get; set; }
    }


}