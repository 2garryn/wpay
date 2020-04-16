using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using wpay.Library.Services.Core.Outbox;
using MassTransit;
using System.Text.Json;

namespace wpay.Library.Services.User2User
{

    public class MessageConsumer: IConsumer<AccountCreated>
    {
        
        public async Task Consume(ConsumeContext<AccountCreated> context)
        {
            var ser = JsonSerializer.Serialize(context.Message);
            Console.WriteLine(ser);
            Console.WriteLine(context.ConversationId);
        }
    }


}