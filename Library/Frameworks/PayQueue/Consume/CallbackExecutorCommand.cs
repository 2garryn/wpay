using System;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using wpay.Library.Frameworks.PayQueue;

namespace wpay.Library.Frameworks.PayQueue.Consume
{
    public class CallbackExecutorCommand<T>: ICallbackExecutor
    {
        private Func<Context, object> _servCreator;
        private DepsCatalog _deps;
        internal CallbackExecutorCommand(Func<Context, object> servCreator, DepsCatalog deps)
        {
            _servCreator = servCreator;
            _deps = deps;
        }

        public async Task Execute(byte[] command, Context context)
        {
            _deps.Logger.LogDebug($"Consume command {typeof(T).FullName}. ID {context.Id}");
            var castedCommand = Deserialize<T>(command);
            var serv = (ICommandConsumer<T>) _servCreator(context);
            await _deps.ErrorCommandHandling().Invoke(context, castedCommand,async () =>
            {
                await serv.ConsumeCommand(castedCommand);
            });
            _deps.Logger.LogDebug($"Consumed command {typeof(T).FullName} successfully. ID {context.Id}");
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