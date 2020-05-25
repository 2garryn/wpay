using System;
using Microsoft.Extensions.Logging;
using wpay.Library.Services.Core.Service;

namespace wpay.Library.Frameworks.PayQueue.Consume
{
    internal class ConsumeCatalogBuilder
    {
        private ConsumeCommandCatalogBuilder _commandCatalog;
        private ConsumeEventCatalogBuilder _eventCatalog;
        private CallbackExecutorFactory _execFactory;
        private readonly DepsCatalog _deps;


        public ConsumeCatalogBuilder(Routes routes, ContextFactory contextFactory, Func<Context, object> servCreator, DepsCatalog deps)
        {
            _commandCatalog = new ConsumeCommandCatalogBuilder(routes, contextFactory);
            _eventCatalog = new ConsumeEventCatalogBuilder(routes, contextFactory);
            _execFactory = new CallbackExecutorFactory(servCreator, deps);
            _deps = deps;
        }


        public void ConsumeCommand<T>()
        {
            _deps.Logger.LogDebug($"Define consume command {typeof(T).FullName}");
            _commandCatalog.Consume<T>(_execFactory.NewCommand<T>());
        }


        public void ConsumeEvent<S, T>() where S : IServiceDefinition, new()  
        {
            _deps.Logger.LogDebug($"Define consume event {typeof(S).FullName}:{typeof(T).FullName}");
            _eventCatalog.Consume<S, T>(_execFactory.NewEvent<S, T>()); 
        }


        public void ConsumeEvent<S, T>(string key) where S : IServiceDefinition, new()
        {
            _deps.Logger.LogDebug($"Define consume event {typeof(S).FullName}:{typeof(T).FullName} with route key {key}");
            _eventCatalog.Consume<S, T>(key, _execFactory.NewEvent<S, T>());
        }
            

        public void Register(IQueueConsumer qConsumer)
        {
            RegisterEvents(qConsumer);
            RegisterCommands(qConsumer);
        }

        private void RegisterCommands(IQueueConsumer qConsumer)
        {
            if (!_commandCatalog.GetConsumeRoute().IsApplicable)
                return;
            _deps.Logger.LogDebug($"Register command queue: {_commandCatalog.GetConsumeRoute().Queue}");
            qConsumer.RegisterCommandConsumer(_commandCatalog.GetConsumeRoute().Queue, _commandCatalog.GetExecuter());
        }
        private void RegisterEvents(IQueueConsumer qConsumer)
        {
            var consumeRoutes = _eventCatalog.GetConsumeRoute();
            if (!consumeRoutes.IsApplicable)
                return;
            var joined = String.Join(", ", consumeRoutes.Exchanges);
            _deps.Logger.LogDebug($"Register event queue: {consumeRoutes.Queue}, exchanges: {joined}");
            qConsumer.RegisterEventConsumer(consumeRoutes.Queue, consumeRoutes.Exchanges, _eventCatalog.GetExecuter());
        }
    }
}