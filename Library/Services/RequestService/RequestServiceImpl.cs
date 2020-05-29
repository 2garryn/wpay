using System;
using System.Threading.Tasks;
using wpay.Library.Frameworks.PayQueue;
using wpay.Library.Frameworks.PayQueue.Publish;
using wpay.Library.Services.ResponseService;

namespace wpay.Library.Services.RequestService
{
    public class RequestServiceImpl: 
        IServiceImpl<RequestServiceDefinition>,
        IEventConsumer<ResponseServiceDefinition, CreatedEvent>,
        IEventConsumer<ResponseServiceDefinition, UpdatedEvent>,
        IEventConsumer<ResponseServiceDefinition, CreateError>
    {

        public async Task Create(Publisher publisher)
        {
            var g = Guid.NewGuid();
            Console.WriteLine($"Start conversation with {g}");
            await publisher.Command<ResponseServiceDefinition, CreateCommand>(new CreateCommand()
            {
                Name = "Some name",
                Amount = 5
            }, ps => ps.ConversationId = g);
        }

        public async Task ConsumeEvent(MessageContext<CreatedEvent> message)
        {
            await message.Publisher.Command<ResponseServiceDefinition, UpdateCommand>(new UpdateCommand()
            {
                Name = "Some update",
                Amount = 4443123,
                Flag = "myflag"
            }, ps => ps.ConversationId = message.ConversationId);
        }
        public async Task ConsumeEvent(MessageContext<UpdatedEvent> message)
        {
            Console.WriteLine($"UpdatedCommand message consumed {message.Message.Name} {message.Message.Amount} {message.Message.Flag}");
            Console.WriteLine($"End convesation with {message.ConversationId}");
            await Task.Yield();
        }

        public async Task ConsumeEvent(MessageContext<CreateError> message)
        {
            Console.WriteLine($"Something goes wrong {message.Message.Reason} convid {message.ConversationId}");
            await Task.Yield();
        }
    }
}