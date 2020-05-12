using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace wpay.Library.Frameworks.PayQueue
{

    public interface IServiceDefinition
    {
        void Configure(IConfigurator conf);
        string Label();
    }

    public interface IConfigurator
    {
        void ConsumeCommand<T>();
        void ConsumeEvent<S, T>() where S: IServiceDefinition, new();
        void ConsumeEvent<S, T>(string key) where S: IServiceDefinition, new();
        void Command<S, T>() where S: IServiceDefinition, new();
        void PublishEvent<T>();
        void PublishEvent<T>(Func<T, string> routeFormatter);
        void PublishError<T>(Func<T, string> routeFormatter);
    }


    public interface IServiceImpl<TServiceDefinition> where TServiceDefinition: IServiceDefinition
    {
    }

    public interface ICommandConsumer<T>
    {
        Task ConsumeCommand(T message);
    }

    public interface IEventConsumer<S, T>
    {
        Task ConsumeEvent(T message);
    }
    public interface IErrorConsumer<S, TError>
    {
        Task ConsumeError(TError message);
    }

    public interface ICallParameters
    {
        Guid? ConversationId {set; get;}
    }

    public delegate Task NextDelegate<T>(T message, Context context);

    public interface IMiddleware
    {
        Task InvokeConsumeCommand<T>(T command, Context context, NextDelegate<T> next);
        Task InvokeConsumeEvent<T>(T ev, Context context, NextDelegate<T> next);
    }

    public interface IErrorHandling
    {
        Task Invoke(Func<Task> next, Func<Task> okClb, Func<object, Task> errorClb);
    }
}