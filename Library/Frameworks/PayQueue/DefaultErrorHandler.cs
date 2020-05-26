using System;
using System.Threading.Tasks;

namespace wpay.Library.Frameworks.PayQueue
{
    public class DefaultErrorHandler: IErrorEventHandling, IErrorCommandHandling
    {
        public async Task Invoke<T>(MessageContext<T> context, Func<MessageContext<T>, Task> act)
        {
            Console.WriteLine("Received message");
            try
            {
                await act(context);
            }
            catch (Exception exc)
            {
                Console.WriteLine($"Something goes wrong {exc.StackTrace}");
            }
        }
    }
}