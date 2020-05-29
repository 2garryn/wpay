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
        Task ConsumeCommand(MessageContext<T> message);
    }

    public interface IEventConsumer<S, T> where S: IServiceDefinition, new()
    {
        Task ConsumeEvent(MessageContext<T>  message);
    }

    public interface ICallParameters
    {
        Guid? ConversationId {set; get;}
        Guid? RequestId { set; get; }
    }



    public interface IErrorCommandHandling
    {
        Task Invoke<TCommand>(MessageContext<TCommand> messageContext, Func<MessageContext<TCommand>, Task> next);
    }
    public interface IErrorEventHandling
    {
        Task Invoke<TEvent>(MessageContext<TEvent> messageContext, Func<MessageContext<TEvent>, Task> next);
    }

    public interface IInternalErrorHandler
    {
        
    }

    public interface IImplFactory<TServDef, TImpl> where TServDef : IServiceDefinition, new() where TImpl : IServiceImpl<TServDef>
    {
        TImpl New();
        IConsumerImplFactory GetConsumerFactory();
    }

}