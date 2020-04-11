using System;
using System.Collections.Generic;
using System.Linq;

namespace wpay.Library.Exceptions
{

    public class WPayException : Exception
    {
        public WPayException(string message) : base(message)
        {
            Info = new Dictionary<string, string>();
        }
        public WPayException(string message, Exception inner) : base(message, inner) 
        { 
            Info = new Dictionary<string, string>(); 
        }
        public WPayException(string message, Dictionary<string, string> info) : base(message) { Info = info; }
        public Dictionary<string, string> Info { get; }
        public override string ToString() => $"{Message}: {Info.ToString()}";
    }


}