using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace wpay.Library.Frameworks.PayQueue
{
    public interface IQueueConsumer
    {
        void RegisterCommandConsumer(string queue, IConsumeExecuter executer);
        void RegisterEventConsumer(string queue, string[] dispatch, IConsumeExecuter executer); 
    }

    public interface IExchangePublisher
    {
        Task PublishEvent(string endpoint, byte[] data);
        Task Command(string endpoint, byte[] data);
    }

    public interface IConsumeExecuter
    {
        Task Execute(IExchangePublisher exchangePublisher, byte[] data);
    }
}