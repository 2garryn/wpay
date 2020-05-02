using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace wpay.Library.Frameworks.PayQueue
{

    public class ExampleServDef : IServiceDefinition
    {
        public string Label() => "exampleserv";
        public void Configure(IConfigurator configurator)
        {
            configurator.ConsumeCommand<MyMessage>();
            configurator.ConsumeEvent<PublServDef, MyMessage4>();
            configurator.PublishEvent<MyMessage2>();
            configurator.PublishEvent<MyMessage3>((ev) => ev.Label());
        }
    }


    public class ExampleServ: 
        IServiceImpl<ExampleServDef>,
        ICommandConsumer<MyMessage>,
        IEventConsumer<PublServDef, MyMessage2>
    {
        public async Task ConsumeCommand(MyMessage message, Context context)
        {

        }
        public async Task ConsumeEvent(MyMessage2 message, Context context)
        {

        }
    }





    


    public class MyMessage {}
    public class MyMessage2 {}
    public class MyMessage3 
    {
        public string Label() => "asda";
    }

    public class MyMessage4 
    {
        public string MyType() => "asda";
    }

    public class PublServDef : IServiceDefinition
    {
        public string Label() => "publeserv";
        public void Configure(IConfigurator configurator)
        {

        }
    }






}