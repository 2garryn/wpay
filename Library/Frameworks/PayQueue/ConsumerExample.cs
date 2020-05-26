using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace wpay.Library.Frameworks.PayQueue
{

    public class RequestServiceDefinition: IServiceDefinition
    {
        public string Label() => "requestservice";
        public void Configure(IConfigurator configurator)
        {
            configurator.Command<RespServDef, Command1>();
            configurator.PublishEvent<Notify1>();
        }
    }
    
    public class RequestServiceImpl:
        IServiceImpl<RequestServiceDefinition>
    {
        private readonly Context _context;
        public RequestServiceImpl(Context context) => _context = context;

        public async Task MakeCommand()
        {
            await _context.Publisher.Command<RespServDef, Command1>(new Command1
            {
                CommandField1 = "haha command"
            });
        }

        public async Task MakeEvent()
        {
            await _context.Publisher.Publish(new Notify1
            {
                NotifyField1 = "haha notifu"
            });
        }
    }

    public class RespServDef: IServiceDefinition
    {
        public string Label() => "responseserv";
        public void Configure(IConfigurator configurator)
        {
            configurator.ConsumeCommand<Command1>();
            configurator.ConsumeEvent<RequestServiceDefinition, Notify1>();
        }
    }

    public class RespServImpl: 
        IServiceImpl<RespServDef>,
        ICommandConsumer<Command1>,
        IEventConsumer<RequestServiceDefinition, Notify1>
    {
        private readonly Context _context;
        public RespServImpl(Context context) => _context = context;

        public async Task ConsumeCommand(MessageContext<Command1> context)
        {
            Console.WriteLine($"Consumed command {context.Message.CommandField1}");
            await Task.Yield();
        }

        public async Task ConsumeEvent(MessageContext<Notify1> notify)
        {
            Console.WriteLine($"Consumed event {notify.Message.NotifyField1}");
            await Task.Yield();
        }
    }


    

    public class Command1
    {
        public string CommandField1 {get;set;}
    }
    public class Notify1
    {
        public string NotifyField1 {get;set;}
    }

}