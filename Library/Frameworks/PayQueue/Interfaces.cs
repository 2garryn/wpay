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
        void InputConsume<T>();
        void InputErrorPublish(string postfix);

        void EventConsume<S, T>() where S: IServiceDefinition, new();
        void EventConsume<S, T>(string key) where S: IServiceDefinition, new();
        void InputSend<S, T>() where S: IServiceDefinition, new();
        void EventPublish<T>();
        void EventPublish<T>(Func<T, string> routeFormatter);
    }


    public interface IServiceImpl<TServiceDefinition> where TServiceDefinition: IServiceDefinition
    {
    }

    public interface IInputConsumer<T>
    {
        Task InputConsume(T message, Context context);
    }

    public interface IEventConsumer<S, T>
    {
        Task EventConsume(T message, Context context);
    }
    public interface IConsumeExecuter
    {
        Task Execute(IExchangePublisher exchangePublisher, byte[] data);
    }



}