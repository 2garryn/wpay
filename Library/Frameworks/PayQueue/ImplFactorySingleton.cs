using System;
using System.Collections.Generic;
using System.Text;

namespace wpay.Library.Frameworks.PayQueue
{
    class ImplFactorySingleton<TServDef, TImpl>
        : IImplFactory<TServDef, TImpl>
        where TServDef : IServiceDefinition, new()
        where TImpl : IServiceImpl<TServDef>
    {
        private TImpl _singleton;
        public ImplFactorySingleton(TImpl singleton)
        {
            _singleton = singleton;
        }


        public TImpl New() => _singleton;

        public IConsumerImplFactory GetConsumerFactory() => new ConsumeImplFactorySingleton<TImpl>(_singleton);
    }

    class ConsumeImplFactorySingleton<TImpl> : IConsumerImplFactory
    {

        private TImpl _singleton;

        public ConsumeImplFactorySingleton(TImpl singleton) => _singleton = singleton;

        public EventConsumerFactory<TServDef, T> NewEventConsumerFactory<TServDef, T>()
            where TServDef : IServiceDefinition, new()
        {
            var consumer = (IEventConsumer<TServDef, T>) _singleton;
            return new EventConsumerFactory<TServDef, T>(() => consumer);
        }

        public CommandConsumerFactory<T> NewCommandConsumerFactory<T>()
        {
            var consumer = (ICommandConsumer<T>)_singleton;
            return new CommandConsumerFactory<T>(() => consumer);
        }
    }
}
