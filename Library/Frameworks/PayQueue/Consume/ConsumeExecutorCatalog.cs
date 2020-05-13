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
        
        public async Task Execute(IExchangePublisher exchangePublisher, byte[] data)
        {
            var (context, t, message) = _contextFactory.New(exchangePublisher, data);
            var executor = _consumers.GetValueOrDefault(t, null!);
            await executor.Execute(message, context);
        }
        

    }


}