using System;
using System.Collections.Generic;
using System.Text;

namespace wpay.Library.Frameworks.PayQueue
{
    class ImplFactoryDelegate<TServDef, TImpl>
        : IImplFactory<TServDef, TImpl>
        where TServDef : IServiceDefinition, new()
        where TImpl : IServiceImpl<TServDef>
    {
        private Func<TImpl> _d;
        public ImplFactoryDelegate(Func<TImpl> d) => _d = d;
        
        public TImpl New() => _d();

        public IConsumerImplFactory GetConsumerFactory() => new ConsumeImplFactoryDelegate<TImpl>(_d);
    }

    class ConsumeImplFactoryDelegate<TImpl> : IConsumerImplFactory
    {
        
        private Func<TImpl> _d;
        public ConsumeImplFactoryDelegate(Func<TImpl> d) => _d = d;

        public EventConsumerFactory<TServDef, T> NewEventConsumerFactory<TServDef, T>()
            where TServDef : IServiceDefinition, new() => 
                new EventConsumerFactory<TServDef, T>(() => (IEventConsumer<TServDef, T>) _d());
        
        public CommandConsumerFactory<T> NewCommandConsumerFactory<T>() => 
            new CommandConsumerFactory<T>(() => (ICommandConsumer<T>) _d());
    }
}