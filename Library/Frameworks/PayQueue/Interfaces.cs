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
        void CommandErrorPublish(string postfix);

        void ConsumeEvent<S, T>() where S: IServiceDefinition, new();
        void ConsumeEvent<S, T>(string key) where S: IServiceDefinition, new();

        void Command<S, T>() where S: IServiceDefinition, new();
        void PublishEvent<T>();
        void PublishEvent<T>(Func<T, string> routeFormatter);
    }


    public interface IServiceImpl<TServiceDefinition> where TServiceDefinition: IServiceDefinition
    {
    }

    public interface ICommandConsumer<T>
    {
        Task ConsumeCommand(T message, Context context);
    }

    public interface IEventConsumer<S, T>
    {
        Task ConsumeEvent(T message, Context context);
    }

    public interface ICallParameters
    {
        Guid? ConversationId {set; get;}
    }



}