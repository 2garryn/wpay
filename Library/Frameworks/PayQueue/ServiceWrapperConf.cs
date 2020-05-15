using System;
using System.Collections.Generic;
using System.Linq;

namespace wpay.Library.Frameworks.PayQueue
{

    public class ServiceWrapperConf
    {
        public string Prefix { get; private set; }
        public Func<IMiddleware> Middleware { get; private set; }
        
        public Func<IErrorCommandHandling> ErrorCommandHandling { get; private set; }
        public Func<IErrorEventHandling> ErrorEventHandling { get; private set; }

        internal ServiceWrapperConf()
        {
            Prefix = "PayQueue";
            Middleware = () => new DefaultMiddleware();
            ErrorCommandHandling = () => new DefaultErrorHandler();
            ErrorEventHandling = () => new DefaultErrorHandler();

        }
        public void UsePrefix(string prefix) => Prefix = prefix;
        public void AddMiddleware(Func<IMiddleware> middleware) => Middleware = middleware;
        public void UseErrorCommandHandling(Func<IErrorCommandHandling> errHandling) => ErrorCommandHandling = errHandling;
        public void UseErrorEventHandling(Func<IErrorEventHandling> errHandling) => ErrorEventHandling = errHandling;
    }



    


}