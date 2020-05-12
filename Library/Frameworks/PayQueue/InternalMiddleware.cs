using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;

namespace wpay.Library.Frameworks.PayQueue
{

    public class InternalMiddleware
    {
        private Func<IMiddleware> _middleware;

        public InternalMiddleware(Func<IMiddleware> middleware)
        {
            _middleware = middleware;
        }

        internal async Task InvokeConsumeCommand<T>(object command, Context context, NextDelegate<T> next)
        {
            await _middleware().InvokeConsumeCommand<T>((T) command, context, next);
        }

        internal async Task InvokeConsumeEvent<S, T>(object ev, Context context, NextDelegate<T> next)
        {
            await _middleware().InvokeConsumeEvent<T>((T) ev, context, next);
        }
    }


}