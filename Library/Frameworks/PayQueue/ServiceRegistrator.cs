using System;
using System.Collections.Generic;
using System.Linq;

namespace wpay.Library.Frameworks.PayQueue
{

    public class ServiceRegistrator
    {
        private IQueueConsumer _consumer;
        private IExchangePublisher _publisher;
        public ServiceRegistrator(IQueueConsumer consumer, IExchangePublisher publisher) => 
            (_consumer, _publisher) = (consumer, publisher);

        public void Register<T>(Func<IServiceImpl<T>> impl) where T: IServiceDefinition, new()
        {


        }
    }


}