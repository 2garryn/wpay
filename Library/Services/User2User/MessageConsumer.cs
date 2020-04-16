using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using wpay.Library.Services.Core.Outbox;
using MassTransit;
using System.Text.Json;
using wpay.Library.Services.Core.Commands;
using wpay.Library.Services.Core.Messages;
using wpay.Library.Services.Core.Models;
using wpay.Library.Models;

namespace wpay.Library.Services.User2User
{

    public class MessageConsumer: IConsumer<AccountCreated>, IConsumer<TransactionCreated>
    {

        public async Task Consume(ConsumeContext<AccountCreated> context)
        {
            //var ser = JsonSerializer.Serialize(context.Message);
            var acc = context.Message.Event.To();
            Console.WriteLine($"Receive created account with {acc.Id.Value.Value} with convId = {context.ConversationId} ");

            var createTran = new CreateTransaction(
                acc.Id,
                new TransactionId(UniqId.New()),
                new TransactionLabel("user2user_source"),
                new CreateAmountCompletedIncome(AmountFactory.New("10.00", CurrencyFactory.New("MXN"))),
                "asdasd"
            );
            await context.Publish(CreateTransactionCommand.From(createTran));
            createTran = new CreateTransaction(
                acc.Id,
                new TransactionId(UniqId.New()),
                new TransactionLabel("user2user_destionation"),
                new CreateAmountCompletedIncome(AmountFactory.New("10.00", CurrencyFactory.New("MXN"))),
                "asdasd"
            );
            await context.Publish(CreateTransactionCommand.From(createTran));

        }
        public async Task Consume(ConsumeContext<TransactionCreated> context)
        {
            //var ser = JsonSerializer.Serialize(context.Message);
            var tr = context.Message.Event.To();
            Console.WriteLine($"Receive created TRansaction with {tr.Id.Value.Value} with convId = {context.ConversationId} with type {tr.Label.Value} ");


        }
    }


}