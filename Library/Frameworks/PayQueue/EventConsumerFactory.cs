
using System;

namespace wpay.Library.Frameworks.PayQueue
{
    public class EventConsumerFactory<S, T> where S: IServiceDefinition, new()
    {

        private Func<IEventConsumer<S, T>> _fact;

        public EventConsumerFactory(Func<IEventConsumer<S, T>> fact) => _fact = fact;

        public IEventConsumer<S, T> New() => _fact();
    }
}