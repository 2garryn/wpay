using System;
using System.Collections.Generic;
using System.Threading.Tasks;
/*
input queue: service_label:input
events queue: service_label:events

input exchange: target_service_label:input
events exchange: producer_service_label:events




*/


namespace wpay.Library.Frameworks.PayQueue
{
    public interface IQueueConsumer
    {
        void RegisterInputConsumer(string queue, Func<IExchangePublisher, byte[], Task> callback);
        void RegisterEventConsumer(string queue, string[] dispatch, Func<IExchangePublisher, byte[], Task> callback); 
    }

    public interface IExchangePublisher
    {
        Task PublishEvent(string endpoint, byte[] data);
        Task PublishInput(string endpoint, byte[] data);
    }

}