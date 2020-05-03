using System;
using System.Collections.Generic;
using System.Linq;

namespace wpay.Library.Frameworks.PayQueue
{

    public class Datagram
    {
        public Datagram() { }
        public Guid Id { get; set; }
        public string Source { get; set; }
        public string Type { get; set; }
        public byte[] Message { get; set; }
        public DatagramOptionalParams OptionalParams { get; set; }
    }

    public class DatagramOptionalParams
    {
        public Guid? ConversationId { get; set; }
    }


}