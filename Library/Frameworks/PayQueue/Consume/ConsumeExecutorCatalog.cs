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
        public ConsumeExecutorCatalog(ImmutableDictionary<string, ICallbackExecutor> consumers, MessageContextFactory contextFactory)
        {
            _consumers = consumers;
            _contextFactory = contextFactory;
        }
        
        public async Task Execute(IExchangePublisher exchangePublisher, string messageType, byte[] data, ConsumeMessageMetadata metadata)
        {
            await GetExecutor(messageType).Execute(exchangePublisher, data);
        }

        private ICallbackExecutor GetExecutor(string t)
        {
            try
            {
                return _consumers[t];
            }
            catch (KeyNotFoundException e)
            {
                var msg = $"Received message was not defined in contract: {t}";
                throw new PayQueueException(msg, e);
            }
        }
        

    }


}