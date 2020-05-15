using System;
using System.Threading.Tasks;
using wpay.Library.Frameworks.PayQueue;
using wpay.Library.Services.Core.Definition;
using wpay.Library.Services.ResponseService;

namespace ResponseService
{
    public class ResponseServiceImpl:    
        IServiceImpl<ResponseServiceDefinition>,
        ICommandConsumer<CreateCommand>,
        ICommandConsumer<UpdateCommand>
    {
        private Context _context;
        public ResponseServiceImpl(Context context) => _context = context;


        public async Task ConsumeCommand(CreateCommand createCommand)
        {
            if (createCommand.Amount < 10)
            {
                throw new Exception("Haha less than 10");
            }
            await _context.Publisher.Publish(new CreatedEvent()
            {
                Name = createCommand.Name,
                Amount = createCommand.Amount
            }, ps => ps.ConversationId = _context.ConversationId);
        }
        public async Task ConsumeCommand(UpdateCommand updateCommand)
        {
            await _context.Publisher.Publish(new UpdatedEvent()
            {
                Name = updateCommand.Name,
                Amount = updateCommand.Amount,
                Flag = updateCommand.Flag
            }, ps => ps.ConversationId = _context.ConversationId);
        }
    }
}