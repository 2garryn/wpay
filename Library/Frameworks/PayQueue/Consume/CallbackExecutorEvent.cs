using System;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using wpay.Library.Frameworks.PayQueue;

namespace wpay.Library.Frameworks.PayQueue.Consume
{
    public class CallbackExecutorEvent<TServDef, T>: ICallbackExecutor where TServDef : IServiceDefinition, new()
    {
        private EventConsumerFactory<TServDef, T> _consumerFactory;
        private DepsCatalog _deps;
        private MessageContextFactory _contextFactory;
        internal CallbackExecutorEvent(EventConsumerFactory<TServDef, T> consumerFactory, MessageContextFactory contextFactory, DepsCatalog deps)
        {
            _consumerFactory = consumerFactory;
            _deps = deps;
            _contextFactory = contextFactory;
        }
        
        public async Task Execute(IExchangePublisher exchangePublisher, byte[] data)
        {
            var messageContext = _contextFactory.New<T>(exchangePublisher, data);
            var impl = _consumerFactory.New();
            await _deps.ErrorEventHandling().Invoke(messageContext, impl.ConsumeEvent);
        }
    }

}