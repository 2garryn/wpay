using System;
using System.Text.Json;
using System.Threading.Tasks;
using wpay.Library.Frameworks.PayQueue;

namespace wpay.Library.Frameworks.PayQueue.Consume
{
    public class CallbackExecutorEvent<TServDef, T>: ICallbackExecutor where TServDef : IServiceDefinition, new()
    {
        private Func<Context, object> _servCreator;
        public CallbackExecutorEvent(Func<Context, object> servCreator)
        {
            _servCreator = servCreator;
        }
        
        public async Task Execute(byte[] ev, Context context)
        {
            var castedEv = JsonSerializer.Deserialize<T>(ev);
            var serv = (IEventConsumer<TServDef, T>) _servCreator(context);
            await serv.ConsumeEvent(castedEv);
        }
    }

}