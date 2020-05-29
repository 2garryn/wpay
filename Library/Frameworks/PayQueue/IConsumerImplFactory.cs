using System;
using System.Collections.Generic;
using System.Text;

namespace wpay.Library.Frameworks.PayQueue
{
    public interface IConsumerImplFactory
    {

        EventConsumerFactory<TServDef, T> NewEventConsumerFactory<TServDef, T>()
            where TServDef : IServiceDefinition, new();

        CommandConsumerFactory<T> NewCommandConsumerFactory<T>();
    }
}
