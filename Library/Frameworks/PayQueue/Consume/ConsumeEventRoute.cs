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
        
        public void Add(Type t, IServiceDefinition servDef, string routeKey = null) =>
            _exchanges.Add(_routes.ConsumeEventExchange(servDef.Label(), t, routeKey));
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