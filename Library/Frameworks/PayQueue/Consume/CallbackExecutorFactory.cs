using System;
using wpay.Library.Frameworks.PayQueue;

namespace wpay.Library.Frameworks.PayQueue.Consume
{
    public class CallbackExecutorFactory
    {
        private Func<Context, object> _servCreator;
        public CallbackExecutorFactory(Func<Context, object> servCreator)
        {
            _servCreator = servCreator;
        }
        public CallbackExecutorCommand<T> NewCommand<T>() =>
            new CallbackExecutorCommand<T>(_servCreator);
        public CallbackExecutorEvent<TServDef, T> NewEvent<TServDef, T>() where TServDef : IServiceDefinition, new() =>
            new CallbackExecutorEvent<TServDef, T>(_servCreator);
    }
}