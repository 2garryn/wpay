using System;

namespace wpay.Library.Frameworks.PayQueue.Consume
{
    public class ConsumeCatalogBuilder
    {
        private ConsumeCommandCatalogBuilder _commandCatalog;
        private ConsumeEventCatalogBuilder _eventCatalog;
        private CallbackExecutorFactory _execFactory;
        private readonly IQueueConsumer _consumer;


        public ConsumeCatalogBuilder(Routes routes, ContextFactory contextFactory, Func<Context, object> servCreator, IQueueConsumer qConsumer, ServiceWrapperConf conf)
        {
            _commandCatalog = new ConsumeCommandCatalogBuilder(routes, contextFactory);
            _eventCatalog = new ConsumeEventCatalogBuilder(routes, contextFactory);
            _execFactory = new CallbackExecutorFactory(servCreator, conf);
            _consumer = qConsumer;

        }


        public void ConsumeCommand<T>() =>
            _commandCatalog.Consume<T>(_execFactory.NewCommand<T>());

        public void ConsumeEvent<S, T>() where S : IServiceDefinition, new()  =>
            _eventCatalog.Consume<S, T>(_execFactory.NewEvent<S, T>());
        
        public void ConsumeEvent<S, T>(string key) where S : IServiceDefinition, new()  =>
            _eventCatalog.Consume<S, T>(key, _execFactory.NewEvent<S, T>());

        public void Register()
        {
            RegisterEvents();
            RegisterCommands();
        }

        private void RegisterCommands()
        {
            if (!_commandCatalog.GetConsumeRoute().IsApplicable)
                return;
            _consumer.RegisterCommandConsumer(_commandCatalog.GetConsumeRoute().Queue, _commandCatalog.GetExecuter());
        }
        private void RegisterEvents()
        {
            var consumeRoutes = _eventCatalog.GetConsumeRoute();
            if (!consumeRoutes.IsApplicable)
                return;
            _consumer.RegisterEventConsumer(consumeRoutes.Queue, consumeRoutes.Exchanges, _eventCatalog.GetExecuter());
        }
    }
}