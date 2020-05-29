using System;

namespace wpay.Library.Frameworks.PayQueue
{
    public class CommandConsumerFactory<T>
    {

        private Func<ICommandConsumer<T>> _fact;

        public CommandConsumerFactory(Func<ICommandConsumer<T>> fact) => _fact = fact;
        public ICommandConsumer<T> New() => _fact();
    }
}