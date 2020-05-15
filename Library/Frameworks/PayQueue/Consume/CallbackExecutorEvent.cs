using System;
using System.Text.Json;
using System.Threading.Tasks;
using wpay.Library.Frameworks.PayQueue;

namespace wpay.Library.Frameworks.PayQueue.Consume
{
    public class CallbackExecutorEvent<TServDef, T>: ICallbackExecutor where TServDef : IServiceDefinition, new()
    {
        private Func<Context, object> _servCreator;
        private ServiceWrapperConf _conf;
        public CallbackExecutorEvent(Func<Context, object> servCreator, ServiceWrapperConf conf)
        {
            _servCreator = servCreator;
            _conf = conf;
        }
        
        public async Task Execute(byte[] ev, Context context)
        {
            var castedEv = Deserialize<T>(ev);
            var serv = (IEventConsumer<TServDef, T>) _servCreator(context);
            await _conf.ErrorEventHandling().Invoke(context, castedEv, async () =>
            {
                await serv.ConsumeEvent(castedEv);
            });
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