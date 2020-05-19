using System;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using wpay.Library.Frameworks.PayQueue;

namespace wpay.Library.Frameworks.PayQueue.Consume
{
    public class CallbackExecutorEvent<TServDef, T>: ICallbackExecutor where TServDef : IServiceDefinition, new()
    {
        private Func<Context, object> _servCreator;
        //private ServiceWrapperConf _conf;
        private DepsCatalog _deps;
        internal CallbackExecutorEvent(Func<Context, object> servCreator, DepsCatalog deps)
        {
            _servCreator = servCreator;
            _deps = deps;
        }
        
        public async Task Execute(byte[] ev, Context context)
        {
            _deps.Logger.LogDebug($"Consume event {typeof(T).FullName}. ID {context.Id}");
            var castedEv = Deserialize<T>(ev);
            var serv = (IEventConsumer<TServDef, T>) _servCreator(context);
            await _deps.ErrorEventHandling().Invoke(context, castedEv, async () =>
            {
                await serv.ConsumeEvent(castedEv);
            });
            _deps.Logger.LogDebug($"Consumed event {typeof(T).FullName} successfully. ID {context.Id}");
        }

        private T Deserialize<T>(byte[] data)
        {
            try
            {
                return JsonSerializer.Deserialize<T>(data);
            }
            catch (JsonException e)
            {
                var asStirng = System.Text.Encoding.UTF8.GetString(data);
                throw new PayQueueException($"Can not deserialize event {typeof(T)}. Data: {asStirng}", e);
            }
        }
    }

}