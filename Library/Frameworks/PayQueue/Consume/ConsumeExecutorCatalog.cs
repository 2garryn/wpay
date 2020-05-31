using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections.Immutable;
using System.Threading.Tasks;
using System.Text.Json;


namespace wpay.Library.Frameworks.PayQueue.Consume
{
    internal class ConsumeExecutorCatalog : IConsumeExecutor
    {
        private readonly ImmutableDictionary<string, ICallbackExecutor> _consumers;
        private readonly MessageContextFactory _contextFactory;
        private readonly DepsCatalog _deps;
        public ConsumeExecutorCatalog(ImmutableDictionary<string, ICallbackExecutor> consumers, MessageContextFactory contextFactory, DepsCatalog depsCatalog)
        {
            _consumers = consumers;
            _contextFactory = contextFactory;
            _deps = depsCatalog;
        }
        
        public async Task Execute(IExchangePublisher exchangePublisher, Func<string> messageType, byte[] data, ConsumeMessageMetadata metadata)
        {
            await GetExecutor(messageType(), metadata).Execute(exchangePublisher, data);
        }
        

        private ICallbackExecutor GetExecutor(string messageType, ConsumeMessageMetadata metadata)
        {
            try
            {
                return _consumers[messageType];
            }
            catch (KeyNotFoundException)
            {
                var excp = new PayQueueException("Received message was not defined in contract.");
                excp.Data["Queue"] = metadata.Queue;
                excp.Data["Exchange"] = metadata.Exchange;
                excp.Data["Type"] = messageType;
                throw excp;
            }
        }
        

    }


}