using System;
using System.Threading.Tasks;

namespace wpay.Library.Frameworks.PayQueue
{
    public class DefaultErrorHandler: IErrorEventHandling, IErrorCommandHandling
    {
        public async Task Invoke<T>(Context context, T message, Func<Task> act)
        {
            Console.WriteLine("Received message");
            try
            {
                act();
            }
            catch (Exception exc)
            {
                Console.WriteLine($"Something goes wrong {exc.StackTrace}");
            }
        }
    }
}