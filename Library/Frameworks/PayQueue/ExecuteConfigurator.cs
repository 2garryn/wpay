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

        internal Action<MessageType, Func<object, Context, Task>> OnInputConsume { get; set; }
        internal Action<MessageType, IServiceDefinition, Func<object, Context, Task>> OnEventConsume { get; set; }
        internal Action<MessageType, IServiceDefinition, string, Func<object, Context, Task>> OnEventConsumeRouted { get; set; }
        internal Action<MessageType> OnEventPublish { get; set; }
        internal Action<MessageType, Func<object, string>> OnEventPublishRouted { get; set; }
        internal Action<MessageType, IServiceDefinition> OnInputSend { get; set; }

        public ExecuteConfigurator(Func<object> servCreator) => _servCreator = servCreator;

        public void InputConsume<T>() =>
            OnInputConsume?.Invoke(typeof(T), async (object val, Context context) =>
            {
                await ((IInputConsumer<T>)_servCreator()).InputConsume((T)val, context);
            });

        public void EventConsume<S, T>() where S : IServiceDefinition, new() =>
            OnEventConsume?.Invoke(typeof(T), new S(), async (object val, Context context) =>
            {
                await ((IEventConsumer<S, T>)_servCreator()).EventConsume((T)val, context);
            });

        public void EventConsume<S, T>(string key) where S : IServiceDefinition, new() =>
            OnEventConsumeRouted?.Invoke(typeof(T), new S(), key, async (object val, Context context) =>
            {
                await ((IEventConsumer<S, T>)_servCreator()).EventConsume((T)val, context);
            });
        
        public void InputErrorPublish(string postfix) { }

        public void InputSend<S, T>() where S : IServiceDefinition, new() => 
            OnInputSend?.Invoke(typeof(T), new S());

        public void EventPublish<T>() => 
            OnEventPublish?.Invoke(typeof(T));

        public void EventPublish<T>(Func<T, string> routeFormatter) =>
            OnEventPublishRouted?.Invoke(typeof(T), (object o) =>
            {
                return routeFormatter((T) o);
            });
    }


}