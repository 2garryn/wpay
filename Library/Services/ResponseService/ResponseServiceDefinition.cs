using System;
using System.Collections.Generic;
using System.Linq;
using wpay.Library.Frameworks.PayQueue;

namespace wpay.Library.Services.ResponseService
{

    public class ResponseServiceDefinition : IServiceDefinition
    {
        public string Label() => "ResponseService";
        public void Configure(IConfigurator conf)
        {
            conf.ConsumeCommand<CreateCommand>();
            conf.ConsumeCommand<UpdateCommand>();
            conf.PublishEvent<CreatedEvent>();
            conf.PublishEvent<CreateError>();
            conf.PublishEvent<UpdatedEvent>((ev) => ev.Flag);
            
        }
    }

    public class CreateCommand
    {
        public string Name { get; set; }
        public int Amount { get; set; }
    }

    public class CreatedEvent
    {
        public string Name { get; set; }
        public int Amount { get; set; }
    }
    
    public class UpdatedEvent
    {
        public string Name { get; set; }
        public int Amount { get; set; }
        public string Flag { get; set; }
    }


    public class UpdateCommand
    {
        public string Name { get; set; }
        public int Amount { get; set; }
        public string Flag { get; set; }
    }

    public class CreateError
    {
        public string Reason { get; set; }
    }


}