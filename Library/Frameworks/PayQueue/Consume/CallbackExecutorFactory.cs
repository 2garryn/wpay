using System;
using wpay.Library.Frameworks.PayQueue;

namespace wpay.Library.Frameworks.PayQueue.Consume
{
    public class CallbackExecutorFactory
    {
        private Func<Context, object> _servCreator;
        private DepsCatalog _deps;
        internal CallbackExecutorFactory(Func<Context, object> servCreator,   DepsCatalog deps)
        {
            _servCreator = servCreator;
            _deps = deps;
        }
        public CallbackExecutorCommand<T> NewCommand<T>() =>
            new CallbackExecutorCommand<T>(_servCreator, _deps);
        public CallbackExecutorEvent<TServDef, T> NewEvent<TServDef, T>() where TServDef : IServiceDefinition, new() =>
            new CallbackExecutorEvent<TServDef, T>(_servCreator, _deps);
    }
}