using System;
using System.Collections.Generic;
using System.Linq;

namespace wpay.Library.Frameworks.PayQueue
{

    public class Datagram
    {
        
        public Datagram() {}

        public Guid ConversationId {get;set;}
        public string Type {get; set;}
        public byte[] Message {get;set;}


    }


}