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


        public async Task ConsumeCommand(MessageContext<CreateCommand> createCommand)
        {

            await createCommand.Publisher.Publish(new CreatedEvent()
            {
                Name = createCommand.Message.Name,
                Amount = createCommand.Message.Amount
            }, ps => ps.ConversationId = createCommand.ConversationId);
        }
        public async Task ConsumeCommand(MessageContext<UpdateCommand> updateCommand)
        {
            await updateCommand.Publisher.Publish(new UpdatedEvent()
            {
                Name = updateCommand.Message.Name,
                Amount = updateCommand.Message.Amount,
                Flag = updateCommand.Message.Flag
            }, ps => ps.ConversationId = updateCommand.ConversationId);
        }
    }
}