namespace wpay.Library.Frameworks.PayQueue
{
    public class ConsumerFactory
    {
        public CommandConsumerFactory<T> NewCommandConsumerFactory<T>()
        {
            return null;
        }

        public EventConsumerFactory<TServDef, T> NewEventConsumerFactory<TServDef, T>()
            where TServDef : IServiceDefinition, new()
        {
            return null;
        }
    }
}