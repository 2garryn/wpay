using System;
using Microsoft.Extensions.Logging;

namespace wpay.Library.Frameworks.PayQueue
{
    internal class DepsCatalog
    {
        public DepsCatalog(ServiceWrapperConf conf, IServiceDefinition def, Type implType)
        {
            Prefix = conf.Prefix ?? "PayQueue";
            MiddlewareCommand = conf.MiddlewareCommand ?? (() => new DefaultMiddleware());
            MiddlewareEvent = conf.MiddlewareEvent ?? (() => new DefaultMiddleware());
            Logger = LoggerFactory.Create(builder =>
                {
                    builder.AddConsole();
                    builder.SetMinimumLevel(LogLevel.Debug);
                })
                .CreateLogger(PrepareCategory(def, implType));
        }


        public string Prefix { get; }
        public Func<IMiddlewareCommand> MiddlewareCommand { get; }
        public Func<IMiddlewareEvent> MiddlewareEvent { get; }
        public ILogger Logger { get; private set; }


        private string PrepareCategory(IServiceDefinition def, Type implType) =>
            $"[{def.GetType().Name}({def.Label()}): {implType.Name}]";

    }
}