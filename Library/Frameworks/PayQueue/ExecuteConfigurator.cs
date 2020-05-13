using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using wpay.Library.Frameworks.PayQueue.Consume;
using wpay.Library.Frameworks.PayQueue.Publish;

namespace wpay.Library.Frameworks.PayQueue
{

    public class ExecuteConfigurator : IConfigurator
    {
        //private readonly CallbackExecutorFactory _execFactory;
        private readonly ConsumeCatalogBuilder _consumeCatalog;
        private readonly PublisherFactoryBuilder _publishCatalog;

        public ExecuteConfigurator(ConsumeCatalogBuilder consumeCatalog, PublisherFactoryBuilder publishCatalog) => 
             (_consumeCatalog, _publishCatalog) = (consumeCatalog, publishCatalog);

        public void ConsumeCommand<T>() =>_consumeCatalog?.ConsumeCommand<T>();
        
        public void ConsumeEvent<S, T>() where S : IServiceDefinition, new() =>
            _consumeCatalog?.ConsumeEvent<S, T>();
        public void ConsumeEvent<S, T>(string key) where S : IServiceDefinition, new() =>
            _consumeCatalog?.ConsumeEvent<S, T>(key);
        public void Command<S, T>() where S : IServiceDefinition, new() =>  _publishCatalog?.Command<S, T>();

        public void PublishEvent<T>() => _publishCatalog?.PublishEvent<T>();

        public void PublishEvent<T>(Func<T, string> routeFormatter) => _publishCatalog?.PublishEvent<T>(routeFormatter);

        public void PublishError<TError>()
        {
        }

        public void PublishError<TError>(Func<TError, string> routeFormatter)
        {
        }

        public void ConsumeError<S, TError>() where S : IServiceDefinition, new()
        {
        }

        public void ConsumeError<S, TError>(string route) where S : IServiceDefinition, new()
        {
        }
    }
}