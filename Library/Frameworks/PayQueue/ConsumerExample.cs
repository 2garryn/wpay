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
            configurator.InputConsume<MyMessage>();
            configurator.EventConsume<PublServDef, MyMessage4>();
            configurator.EventPublish<MyMessage2>();
            configurator.EventPublish<MyMessage3>((ev) => ev.Label());
        }
    }


    public class ExampleServ: 
        IServiceImpl<ExampleServDef>,
        IInputConsumer<MyMessage>,
        IEventConsumer<PublServDef, MyMessage2>
    {
        public async Task InputConsume(MyMessage message, Context context)
        {

        }
        public async Task EventConsume(MyMessage2 message, Context context)
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