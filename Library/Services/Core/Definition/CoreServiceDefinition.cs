using System;
using System.Collections.Generic;
using System.Linq;
using wpay.Library.Frameworks.PayQueue;

namespace wpay.Library.Services.Core.Definition
{

    public class CoreServiceDefinition: IServiceDefinition
    {
        public string Label() => "core_service";
        public void Configure(IConfigurator configurator)
        {
            configurator.ConsumeCommand<CreateAccountCommand>();
            configurator.ConsumeCommand<CreateTransactionCommand>();
            configurator.ConsumeCommand<UpdateTransactionCommand>();

            configurator.PublishEvent<AccountCreated>();
            configurator.PublishEvent<AccountLocked>();
            configurator.PublishEvent<AccountUnlocked>();
            configurator.PublishEvent<TransactionCreated>((ev) => ev.Event.Label);
            configurator.PublishEvent<TransactionUpdated>((ev) => ev.Event.Label);
        }
    }



}