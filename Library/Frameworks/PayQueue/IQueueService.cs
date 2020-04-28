using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace wpay.Library.Frameworks.PayQueue
{
    public interface IQueueConsumer
    {
        void RegisterInput(string endpointPath, Func<IPublisher, byte[], Task> callback);
        void RegisterEvent(string endpointPath, Func<IPublisher, byte[], Task> callback); 
    }

    public interface IPublisher
    {
        Task PublishEvent(string endpoint, byte[] data);
        Task PublishInput(string endpoint, byte[] data);
    }

}