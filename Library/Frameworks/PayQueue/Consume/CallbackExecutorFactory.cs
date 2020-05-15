using System;
using wpay.Library.Frameworks.PayQueue;

namespace wpay.Library.Frameworks.PayQueue.Consume
{
    public class CallbackExecutorFactory
    {
        private Func<Context, object> _servCreator;
        private ServiceWrapperConf _conf;
        public CallbackExecutorFactory(Func<Context, object> servCreator, ServiceWrapperConf conf)
        {
            _servCreator = servCreator;
            _conf = conf;
        }
        public CallbackExecutorCommand<T> NewCommand<T>() =>
            new CallbackExecutorCommand<T>(_servCreator, _conf);
        public CallbackExecutorEvent<TServDef, T> NewEvent<TServDef, T>() where TServDef : IServiceDefinition, new() =>
            new CallbackExecutorEvent<TServDef, T>(_servCreator, _conf);
    }
}