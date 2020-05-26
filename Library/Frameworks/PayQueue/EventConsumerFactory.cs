namespace wpay.Library.Frameworks.PayQueue
{
    public class EventConsumerFactory<S, T> where S: IServiceDefinition, new()
    {
        public IEventConsumer<S, T> New()
        {
            return null;
        }
    }
}