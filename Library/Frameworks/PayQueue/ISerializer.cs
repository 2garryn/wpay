using System;
using System.Collections.Generic;

namespace wpay.Library.Frameworks.PayQueue
{

    public interface ISerializer
    {
        Datagram Deserialize(byte[] data);
        
    }


}