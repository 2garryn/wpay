using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Immutable;
using wpay.Library.Frameworks.PayQueue.Consume;
using wpay.Library.Frameworks.PayQueue.Publish;


namespace wpay.Library.Frameworks.PayQueue
{
    using MessageType = Type;
    using CallbackAction = Func<object, Context, Task>;

    public delegate TImpl ImplFactory<TImpl, TServDef>(Context context)
            where TServDef : IServiceDefinition, new()
            where TImpl : IServiceImpl<TServDef>;


    public class ServiceWrapper<TServDef, TImpl>
        where TServDef : IServiceDefinition, new()
        where TImpl : IServiceImpl<TServDef>
    {
        private readonly IQueueConsumer _consumer;
        private readonly ImplFactory<TImpl, TServDef> _impl;
        private PublisherFactory _publisherFactory;
        private readonly ServiceWrapperConf _conf;


        public ServiceWrapper(IQueueConsumer consumer, ImplFactory<TImpl, TServDef> impl, Action<ServiceWrapperConf>? confAct = null) 
        {
            _consumer = consumer;
            _impl = impl;
            _conf = new ServiceWrapperConf();
            confAct?.Invoke(_conf);
        }

        public void Execute()
        {
            var def = new TServDef();
            var routes = new Routes(_conf.Prefix, new TServDef().Label());
            var publFactoryBuilder = new PublisherFactoryBuilder(routes);
            def.Configure(new ExecuteConfigurator(null!, publFactoryBuilder));
            _publisherFactory = publFactoryBuilder.Build();
            var contextFactory = new ContextFactory(_publisherFactory);
            var consumeCatalog = new ConsumeCatalogBuilder(routes, contextFactory, (context) => _impl(context), _consumer);
            consumeCatalog.Register();
        }

        public TImpl GetService(Action<ICallParameters>? p = null)
        {
            var callParameters = new CallParameters();
            p?.Invoke(callParameters);

            var context = new Context(
                Guid.NewGuid(),
                callParameters.ConversationId,
                _publisherFactory.New(_consumer.GetExchangePublisher())
            );
            return _impl(context);
        }
        
    }
}