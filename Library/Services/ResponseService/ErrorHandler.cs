using System;
using System.Threading.Tasks;
using wpay.Library.Frameworks.PayQueue;
using wpay.Library.Services.ResponseService;

namespace wpay.Library.Services.ResponseService
{
    public class ErrorHandler: IErrorEventHandling, IErrorCommandHandling
    {
        public async Task Invoke<T>(Context context, T message, Func<Task> task)
        {
            try
            {
                await task();
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