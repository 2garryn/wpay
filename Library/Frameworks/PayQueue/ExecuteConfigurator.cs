using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace wpay.Library.Frameworks.PayQueue
{
    using MessageType = Type;
    using ErrorType = Type;

    internal delegate void OnConsumeCommandDelegate(MessageType messageType, Func<object, Context, Task> clb);

    internal delegate void OnConsumeEventDelegate(MessageType messageType, IServiceDefinition servDef, 
        Func<object, Context, Task> clb);

    internal delegate void OnConsumeEventRoutedDelegate(MessageType messageType, IServiceDefinition servDef, 
        string route, Func<object, Context, Task> clb);

    internal delegate void OnPublishEventDelegate(MessageType messageType);

    internal delegate void OnPublishEventRoutedDelegate(MessageType messageType, Func<object, string> routeClb);

    internal delegate void OnCommandDelegate(MessageType messageType, IServiceDefinition servDef);

    internal delegate void OnPublishErrorDelegate(ErrorType errorType);

    internal delegate void OnPublishErrorRoutedDelegate(ErrorType errorType, Func<object, string> routeClb);

    internal delegate void OnConsumeErrorDelegate(ErrorType errorType, IServiceDefinition servDef, Func<object, Context, Task> clb);

    internal delegate void OnConsumeErrorRoutedDelegate(ErrorType errorType, IServiceDefinition servDef,
        string route, Func<object, Context, Task> clb);

    public class ExecuteConfigurator : IConfigurator
    {
        private readonly Func<Context, object> _servCreator;
        private readonly InternalMiddleware _middleware;

        internal OnConsumeCommandDelegate? OnConsumeCommand { get; set; }
        internal OnConsumeEventDelegate? OnConsumeEvent { get; set; }
        internal OnConsumeEventRoutedDelegate? OnConsumeEventRouted { get; set; }
        internal OnPublishEventDelegate? OnPublishEvent { get; set; }
        internal OnPublishEventRoutedDelegate? OnPublishEventRouted { get; set; }
        internal OnCommandDelegate? OnCommand { get; set; }
        internal OnPublishErrorDelegate? OnPublishError {get;set;}
        internal OnPublishErrorRoutedDelegate? OnPublishErrorRouted {get;set;}
        internal OnConsumeErrorDelegate? OnConsumeError {get;set;}
        internal OnConsumeErrorRoutedDelegate? OnConsumeErrorRouted {get;set;}

        public ExecuteConfigurator(Func<Context, object> servCreator, InternalMiddleware middleware) => 
            (_servCreator, _middleware) = (servCreator, middleware);

        public void ConsumeCommand<T>() =>
            OnConsumeCommand?.Invoke(typeof(T), async (object command, Context context) =>
            {
                await _middleware.InvokeConsumeCommand<T>(command, context, async (T castedCommand, Context context) =>
                    await ((ICommandConsumer<T>) _servCreator(context)).ConsumeCommand(castedCommand));
            });

        public void ConsumeEvent<S, T>() where S : IServiceDefinition, new() =>
            OnConsumeEvent?.Invoke(typeof(T), new S(), async (object ev, Context context) =>
            {
                await _middleware.InvokeConsumeEvent<S, T>(ev, context, async (T castedEv, Context context) =>
                    await ((IEventConsumer<S, T>)_servCreator(context)).ConsumeEvent(castedEv));
            });

        public void ConsumeEvent<S, T>(string key) where S : IServiceDefinition, new() =>
            OnConsumeEventRouted?.Invoke(typeof(T), new S(), key, async (object ev, Context context) =>
            {
                await _middleware.InvokeConsumeEvent<S, T>(ev, context, async (T castedEv, Context context) =>
                    await ((IEventConsumer<S, T>)_servCreator(context)).ConsumeEvent(castedEv));
            });

        public void Command<S, T>() where S : IServiceDefinition, new() => 
            OnCommand?.Invoke(typeof(T), new S());

        public void PublishEvent<T>() => 
            OnPublishEvent?.Invoke(typeof(T));

        public void PublishEvent<T>(Func<T, string> routeFormatter) =>
            OnPublishEventRouted?.Invoke(typeof(T), (object o) =>
            {
                return routeFormatter((T) o);
            });

        public void PublishError<TError>(Func<TError, string> routeFormatter) =>
            OnPublishErrorRouted?.Invoke(typeof(TError), (object o) =>
            {
                return routeFormatter((TError) o);
            });


        public void PublishError<TError>() =>
            OnPublishError?.Invoke(typeof(TError));
        

        public void ConsumeError<S, TError>() where S : IServiceDefinition, new() => 
            OnConsumeError?.Invoke(typeof(TError), new S(), async (object o, Context context) => 
            {
                await ((IErrorConsumer<S, TError>)_servCreator(context)).ConsumeError((TError) o);
            });
        
        public void ConsumeError<S, TError>(string route) where S : IServiceDefinition, new() => 
            OnConsumeErrorRouted?.Invoke(typeof(TError), new S(), route,  async (object o, Context context) => 
            {
                await ((IErrorConsumer<S, TError>)_servCreator(context)).ConsumeError((TError) o);
            });
    }
}