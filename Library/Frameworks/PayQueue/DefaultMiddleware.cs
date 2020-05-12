using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace wpay.Library.Frameworks.PayQueue
{

    public class DefaultMiddleware: IMiddleware
    {
        public async Task InvokeConsumeCommand<T>(T command, Context context, NextDelegate<T> next) =>
            await next(command, context);
        public async Task InvokeConsumeEvent<T>(T ev, Context context, NextDelegate<T> next) => 
            await next(ev, context);
    }


}