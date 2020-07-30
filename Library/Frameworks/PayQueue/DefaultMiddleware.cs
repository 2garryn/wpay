using System;
using System.Threading.Tasks;

namespace wpay.Library.Frameworks.PayQueue
{
    public class DefaultMiddleware: IMiddlewareEvent, IMiddlewareCommand
    {
        public async Task Invoke<T>(MessageContext<T> context, Func<MessageContext<T>, Task> next) => await next(context);
    }
}