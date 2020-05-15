using System;
using System.Threading.Tasks;
using wpay.Library.Frameworks.PayQueue;
using wpay.Library.Services.ResponseService;

namespace wpay.Library.Services.RequestService
{
    public class RequestServiceImpl: 
        IServiceImpl<RequestServiceDefinition>,
        IEventConsumer<ResponseServiceDefinition, CreatedEvent>,
        IEventConsumer<ResponseServiceDefinition, UpdatedEvent>,
        IEventConsumer<ResponseServiceDefinition, CreateError>
    {
        private Context _context;
        public RequestServiceImpl(Context context)
        {
            _context = context;
        }

        public async Task Create()
        {
            var g = Guid.NewGuid();
            Console.WriteLine($"Start conversation with {g}");
            await _context.Publisher.Command<ResponseServiceDefinition, CreateCommand>(new CreateCommand()
            {
                Name = "Some name",
                Amount = 5
            }, ps => ps.ConversationId = g);
        }

        public async Task ConsumeEvent(CreatedEvent message)
        {
            await _context.Publisher.Command<ResponseServiceDefinition, UpdateCommand>(new UpdateCommand()
            {
                Name = "Some update",
                Amount = 4443123,
                Flag = "myflag"
            }, ps => ps.ConversationId = _context.ConversationId);
        }
        public async Task ConsumeEvent(UpdatedEvent message)
        {
            Console.WriteLine($"UpdatedCommand message consumed {message.Name} {message.Amount} {message.Flag}");
            Console.WriteLine($"End convesation with {_context.ConversationId}");
            await Task.Yield();
        }

        public async Task ConsumeEvent(CreateError message)
        {
            Console.WriteLine($"Something goes wrong {message.Reason} convid {_context.ConversationId}");
            await Task.Yield();
        }
    }
}