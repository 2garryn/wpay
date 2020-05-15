using System;
using System.Collections.Generic;
using System.Linq;

namespace wpay.Library.Frameworks.PayQueue.Consume
{
    public class ConsumeEventRoute
    {
        private Routes _routes;
        private HashSet<string> _exchanges;
        public ConsumeEventRoute(Routes routes)
        {
            _routes = routes;
            _exchanges = new HashSet<string>();
        }
        
        public void Add<S, T>( string routeKey = null) where S : IServiceDefinition, new() =>
            _exchanges.Add(_routes.ConsumeEventExchange(new S().Label(), typeof(T), routeKey));
        public string Queue
        {
            get { return _routes.ConsumeEventQueue(); }
        }
        public string[] Exchanges
        {
            get { return _exchanges.ToArray();  }
        }
        public bool IsApplicable
        {
            get { return _exchanges.Count() > 0; }
        }
    }
}