using System;
using System.Threading.Tasks;
using wpay.Library.Frameworks.PayQueue;
using wpay.Library.Services.ResponseService;

namespace wpay.Library.Services.ResponseService
{
    public class ErrorHandler: IErrorEventHandling, IErrorCommandHandling
    {
        public async Task Invoke<T>(MessageContext<T> context, Func<MessageContext<T>, Task> next)
        {
            try
            {
                await next(context);
            }
            catch (Exception excp)
            {
                await context.Publisher.Publish(new CreateError()
                {
                    Reason = excp.Message,
                }, ps => ps.ConversationId = context.ConversationId);
            }
        }
    }
}