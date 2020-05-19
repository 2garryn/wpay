using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;

namespace wpay.Library.Frameworks.PayQueue
{

    public class ServiceWrapperConf
    {
        internal string Prefix { get; private set; }
        internal Func<IMiddleware> Middleware { get; private set; }
        internal Func<IErrorCommandHandling> ErrorCommandHandling { get; private set; }
        internal Func<IErrorEventHandling> ErrorEventHandling { get; private set; }
        internal ILogger Logger { get; private set; }
        
        public void UsePrefix(string prefix) => Prefix = prefix;
        public void AddMiddleware(Func<IMiddleware> middleware) => Middleware = middleware;
        public void UseErrorCommandHandling(Func<IErrorCommandHandling> errHandling) => ErrorCommandHandling = errHandling;
        public void UseErrorEventHandling(Func<IErrorEventHandling> errHandling) => ErrorEventHandling = errHandling;
        public void UseLogger(ILogger logger) => Logger = logger;
    }



    


}