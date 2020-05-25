using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace wpay.Library.Frameworks.PayQueue
{
    public interface IQueueConsumer
    {
        void RegisterCommandConsumer(string queue, IConsumeExecutor executor);
        void RegisterEventConsumer(string queue, string[] dispatch, IConsumeExecutor executor); 
        IExchangePublisher GetExchangePublisher();
    }

    public interface IExchangePublisher
    {
        Task PublishEvent(string endpoint, byte[] data);
        Task Command(string endpoint, byte[] data);
    }

    public interface IConsumeExecutor
    {
        Task Execute(IExchangePublisher exchangePublisher, ConsumeMessageMetadata messageMetadata, byte[] data);
    }


    public class  ConsumeMessageMetadata
    {
        public string? Queue {get;set;}
        public string? Exchange {get;set;}
    }
    
}