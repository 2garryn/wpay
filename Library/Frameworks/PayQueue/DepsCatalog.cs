using System;
using Microsoft.Extensions.Logging;

namespace wpay.Library.Frameworks.PayQueue
{
    internal class DepsCatalog
    {
        public DepsCatalog(ServiceWrapperConf conf, IServiceDefinition def, Type implType)
        {
            Prefix = conf.Prefix ?? "PayQueue";
            Middleware = conf.Middleware ?? (() => new DefaultMiddleware());
            ErrorCommandHandling = conf.ErrorCommandHandling ?? (() => new DefaultErrorHandler());
            ErrorEventHandling = conf.ErrorEventHandling ?? (() => new DefaultErrorHandler());
            Logger = LoggerFactory.Create(builder =>
                {
                    builder.AddConsole();
                    builder.SetMinimumLevel(LogLevel.Debug);
                })
                .CreateLogger(PrepareCategory(def, implType));
        }
        
        
        public string Prefix { get; }
        public Func<IMiddleware> Middleware { get;  }
        public Func<IErrorCommandHandling> ErrorCommandHandling { get; }
        public Func<IErrorEventHandling> ErrorEventHandling { get;  }
        public ILogger Logger { get; private set; }


        private string PrepareCategory(IServiceDefinition def, Type implType) => 
            $"[{def.GetType().Name}({def.Label()}): {implType.Name}]";
        
    }
}