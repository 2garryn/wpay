using System;
using System.Text.Json;
using System.Threading.Tasks;
using wpay.Library.Frameworks.PayQueue;

namespace wpay.Library.Frameworks.PayQueue.Consume
{
    public class CallbackExecutorCommand<T>: ICallbackExecutor
    {
        private Func<Context, object> _servCreator;
        private ServiceWrapperConf _conf;
        //private readonly InternalMiddleware _middleware;
        public CallbackExecutorCommand(Func<Context, object> servCreator, ServiceWrapperConf conf)
        {
            _servCreator = servCreator;
            _conf = conf;
        }

        public async Task Execute(byte[] command, Context context)
        {
            var castedCommand = Deserialize<T>(command);
            var serv = (ICommandConsumer<T>) _servCreator(context);
            await _conf.ErrorCommandHandling().Invoke(context, castedCommand,async () =>
            {
                await serv.ConsumeCommand(castedCommand);
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
                throw new PayQueueException($"Can not deserialize command {typeof(T)}. Data: {asStirng}", e);
            }
        }
    }
}