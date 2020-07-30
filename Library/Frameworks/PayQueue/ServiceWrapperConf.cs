using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;

namespace wpay.Library.Frameworks.PayQueue
{

    public class ServiceWrapperConf
    {
        internal string Prefix { get; private set; }
        internal string ErrorPostfix {get; private set;}
        //internal Func<IMiddleware> Middleware { get; private set; }
        internal Func<IMiddlewareCommand> MiddlewareCommand { get; private set; }
        internal Func<IMiddlewareEvent> MiddlewareEvent { get; private set; }
        internal ILogger Logger { get; private set; }
        
        public void UsePrefix(string prefix) => Prefix = prefix;
        //public void AddMiddleware(Func<IMiddleware> middleware) => Middleware = middleware;
        public void UseMiddlewareCommand(Func<IMiddlewareCommand> middleware) => MiddlewareCommand = middleware;
        public void UseMiddlewareEvent(Func<IMiddlewareEvent> middleware) => MiddlewareEvent = middleware;
        public void UseLogger(ILogger logger) => Logger = logger;
    
    }



    


}