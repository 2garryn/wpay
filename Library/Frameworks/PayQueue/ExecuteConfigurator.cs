using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace wpay.Library.Frameworks.PayQueue
{
    using MessageType = Type;
    public class ExecuteConfigurator : IConfigurator
    {
        private readonly Func<object> _servCreator;

        internal Action<MessageType, Func<object, Context, Task>> OnConsumeCommand { get; set; }
        internal Action<MessageType, IServiceDefinition, Func<object, Context, Task>> OnConsumeEvent { get; set; }
        internal Action<MessageType, IServiceDefinition, string, Func<object, Context, Task>> OnConsumeEventRouted { get; set; }
        internal Action<MessageType> OnPublishEvent { get; set; }
        internal Action<MessageType, Func<object, string>> OnPublishEventRouted { get; set; }
        internal Action<MessageType, IServiceDefinition> OnCommand { get; set; }

        public ExecuteConfigurator(Func<object> servCreator) => _servCreator = servCreator;

        public void ConsumeCommand<T>() =>
            OnConsumeCommand?.Invoke(typeof(T), async (object val, Context context) =>
            {
                await ((ICommandConsumer<T>)_servCreator()).ConsumeCommand((T)val, context);
            });

        public void ConsumeEvent<S, T>() where S : IServiceDefinition, new() =>
            OnConsumeEvent?.Invoke(typeof(T), new S(), async (object val, Context context) =>
            {
                await ((IEventConsumer<S, T>)_servCreator()).ConsumeEvent((T)val, context);
            });

        public void ConsumeEvent<S, T>(string key) where S : IServiceDefinition, new() =>
            OnConsumeEventRouted?.Invoke(typeof(T), new S(), key, async (object val, Context context) =>
            {
                await ((IEventConsumer<S, T>)_servCreator()).ConsumeEvent((T)val, context);
            });
        
        public void CommandErrorPublish(string postfix) { }

        public void Command<S, T>() where S : IServiceDefinition, new() => 
            OnCommand?.Invoke(typeof(T), new S());

        public void PublishEvent<T>() => 
            OnPublishEvent?.Invoke(typeof(T));

        public void PublishEvent<T>(Func<T, string> routeFormatter) =>
            OnPublishEventRouted?.Invoke(typeof(T), (object o) =>
            {
                return routeFormatter((T) o);
            });
    }


}