using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Immutable;
using wpay.Library.Frameworks.PayQueue.Consume;
using wpay.Library.Frameworks.PayQueue.Publish;


namespace wpay.Library.Frameworks.PayQueue
{

    public class ServiceWrapper<TServDef, TImpl>
        where TServDef : IServiceDefinition, new()
        where TImpl : IServiceImpl<TServDef>
    {
        private readonly IQueueConsumer _consumer;
        private readonly IImplFactory<TServDef, TImpl> _impl;
        private PublisherFactory _publisherFactory;

        private readonly  DepsCatalog _deps;
        //private readonly ServiceWrapperConf _conf;


        public ServiceWrapper(IQueueConsumer consumer, IImplFactory<TServDef, TImpl> impl, Action<ServiceWrapperConf>? confAct = null) 
        {
            _consumer = consumer;
            _impl = impl;
            var conf = new ServiceWrapperConf();
            confAct?.Invoke(conf);
            _deps = new DepsCatalog(conf, new TServDef(), typeof(TImpl));
        }

        public void Execute()
        {
            var def = new TServDef();
            var routes = new Routes(_deps.Prefix, def.Label());
            var publFactoryBuilder = new PublisherFactoryBuilder(routes, _deps, def.Label(), "none");
            def.Configure(new ExecuteConfigurator(null!, publFactoryBuilder));
            _publisherFactory = publFactoryBuilder.Build();
            var contextFactory = new MessageContextFactory(_publisherFactory);
            var consumeCatalog = new ConsumeCatalogBuilder(routes, contextFactory, _impl.GetConsumerFactory(), _deps);
            def.Configure(new ExecuteConfigurator(consumeCatalog, null!));
            consumeCatalog.Register(_consumer);
        }


        public async Task<TResult> CallAsync<TResult>(Func<TImpl, Publisher, Task<TResult>> caller)
        {
            var publ = _publisherFactory.New(_consumer.GetExchangePublisher());
            return await caller(_impl.New(), publ);
        }
        
        public async Task CallAsync(Func<TImpl, Publisher, Task> caller)
        {
            var publ = _publisherFactory.New(_consumer.GetExchangePublisher());
            await caller(_impl.New(), publ);
        }

        
    }
}