using System;
using System.Text.Json;
using System.Threading.Tasks;
using MassTransit.Audit;
using Microsoft.Extensions.Logging;
using wpay.Library.Frameworks.PayQueue;

namespace wpay.Library.Frameworks.PayQueue.Consume
{
    public class CallbackExecutorCommand<T>: ICallbackExecutor
    {
        private CommandConsumerFactory<T> _consumerFactory;
        private DepsCatalog _deps;
        private MessageContextFactory _contextFactory;
        internal CallbackExecutorCommand(CommandConsumerFactory<T> consumerFactory, MessageContextFactory contextFactory, DepsCatalog deps)
        {
            _consumerFactory = consumerFactory;
            _deps = deps;
            _contextFactory = contextFactory;
        }

        public async Task Execute(IExchangePublisher exchangePublisher, byte[] data)
        {
            var messageContext = _contextFactory.New<T>(exchangePublisher, data);
            var impl = _consumerFactory.New();
            await _deps.ErrorCommandHandling().Invoke(messageContext, impl.ConsumeCommand);
        }
    }
}