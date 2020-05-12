using System;
using System.Collections.Generic;
using System.Linq;

namespace wpay.Library.Frameworks.PayQueue
{

    public class ServiceWrapperConf
    {
        public string Prefix { get; private set; }
        public Func<IMiddleware> Middleware { get; private set; }

        internal ServiceWrapperConf()
        {
            Prefix = "PayQueue";
            Middleware = () => new DefaultMiddleware();

        }
        public void UsePrefix(string prefix) => Prefix = prefix;
        public void AddMiddleware(Func<IMiddleware> middleware) => Middleware = middleware;
        public void UseErrorHandling(Func<IErrorHandling> errHandling) {}
    }



    


}