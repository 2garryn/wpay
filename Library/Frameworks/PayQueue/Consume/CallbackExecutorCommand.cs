using System;
using System.Text.Json;
using System.Threading.Tasks;
using wpay.Library.Frameworks.PayQueue;

namespace wpay.Library.Frameworks.PayQueue.Consume
{
    public class CallbackExecutorCommand<T>: ICallbackExecutor
    {
        private Func<Context, object> _servCreator;
        //private readonly InternalMiddleware _middleware;
        public CallbackExecutorCommand(Func<Context, object> servCreator)
        {
            _servCreator = servCreator;
        }

        public async Task Execute(byte[] command, Context context)
        {
            var castedCommand = JsonSerializer.Deserialize<T>(command); 
            var serv = (ICommandConsumer<T>) _servCreator(context);
            await serv.ConsumeCommand(castedCommand);
        }
    }
}