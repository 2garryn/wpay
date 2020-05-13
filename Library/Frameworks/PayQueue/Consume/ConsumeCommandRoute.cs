namespace wpay.Library.Frameworks.PayQueue.Consume
{
    public class ConsumeCommandRoute
    {
        private readonly Routes _routes;
        public ConsumeCommandRoute(Routes routes) => _routes = routes;
        public string Queue
        {
            get { return _routes.ConsumeCommandQueue(); }
        }
        public bool IsApplicable
        {
            get { return true; }
        }
    }
}