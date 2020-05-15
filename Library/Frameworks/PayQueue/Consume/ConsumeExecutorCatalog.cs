using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections.Immutable;
using System.Threading.Tasks;
using System.Text.Json;


namespace wpay.Library.Frameworks.PayQueue.Consume
{
    public class ConsumeExecutorCatalog : IConsumeExecutor
    {
        private readonly ImmutableDictionary<Type, ICallbackExecutor> _consumers;
        private readonly ContextFactory _contextFactory;
        public ConsumeExecutorCatalog(ImmutableDictionary<Type, ICallbackExecutor> consumers, ContextFactory contextFactory)
        {
            _consumers = consumers;
            _contextFactory = contextFactory;
        }
        
        public async Task Execute(IExchangePublisher exchangePublisher, Dictionary<string, string> metadata, byte[] data)
        {
            var (context, t, message) = _contextFactory.New(exchangePublisher, data);
            await GetExecutor(t).Execute(message, context);
        }

        private ICallbackExecutor GetExecutor(Type t)
        {
            try
            {
                return _consumers[t];
            }
            catch (KeyNotFoundException e)
            {
                var msg = $"Received message was not defined in contract: {t.FullName}";
                throw new PayQueueException(msg, e);
            }
        }
        

    }


}