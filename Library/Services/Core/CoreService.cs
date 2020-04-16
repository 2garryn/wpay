using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;
using Microsoft.Extensions.Configuration;
using wpay.Library.Services.Core.Commands;
using wpay.Library.Services.Core;
using wpay.Library.Libs.Db;
using wpay.Library.Services.Core.Models;
using wpay.Library.Services.Core.Messages;
using wpay.Library.Services.Core.Outbox;
using wpay.Library.Exceptions;
using MassTransit;

namespace wpay.Library.Services.Core
{

    public class CoreService
    {
        private readonly DbClient _db;
        private readonly string _rabbitHost;
        private readonly string _rabbitUsername;
        private readonly string _rabbitPassword;
        private readonly string _rabbitEndpoint;
        public CoreService(IConfiguration conf)
        {
            var dbSection = conf.GetSection("database");
            _db = new DbClient(dbSection["host"], dbSection["username"], dbSection["password"], dbSection["database"]);

            var rabbitSection = conf.GetSection("rabbitmq");

            (_rabbitHost, _rabbitUsername, _rabbitPassword, _rabbitEndpoint) =
                (rabbitSection["host"], rabbitSection["username"], rabbitSection["password"], rabbitSection["endpoint"]);
        }

        public async Task Execute()
        {

            var bus = Bus.Factory.CreateUsingRabbitMq(sbc =>
            {
                sbc.Host(new Uri(_rabbitHost), host =>
                {
                    host.Username(_rabbitUsername);
                    host.Password(_rabbitPassword);
                });
                sbc.ReceiveEndpoint(_rabbitEndpoint, ep =>
                {
                    ep.Consumer(() => new MessageConsumer(_db));
                });
            });
            await bus.StartAsync(); // This is important!
        }

        public class MessageConsumer :
            IConsumer<CreateAccountCommand>,
            IConsumer<CreateTransactionCommand>,
            IConsumer<UpdateTransactionCommand>
        {

            private readonly DbClient _db;
            public MessageConsumer(DbClient db)
            {
                _db = db;
            }

            public async Task Consume(ConsumeContext<CreateAccountCommand> context) =>
                await Exec(async (core) =>
                {
                    var options = new CreateAccountOptions()
                    {
                        IngoreOnDuplicate = true
                    };
                    var acc = await core.CreateAsync(context.Message.To(), options);
                    return new AccountCreated() { Event = AccountEvent.From(acc) };
                }, context.ConversationId);

            public async Task Consume(ConsumeContext<CreateTransactionCommand> context) =>
                await Exec(async (core) =>
                {
                    var options = new CreateTransactionOptions()
                    {
                        FailOnExist = false
                    };
                    var tran = await core.CreateAsync(context.Message.To(), options);
                    return new TransactionCreated() { Event = TransactionEvent.From(tran) };
                },
                context.ConversationId);

            public async Task Consume(ConsumeContext<UpdateTransactionCommand> context) =>
                await Exec(async (core) =>
                {
                    var options = new UpdateTransactionOptions()
                    {
                        FailOnUpdateDone = false
                    };
                    var tran = await core.UpdateAsync(context.Message.To(), options);
                    return new TransactionUpdated() { Event = TransactionEvent.From(tran) };
                },
                context.ConversationId);

            private async Task Exec(Func<Service.Service, Task<ICoreEvent>> serv, Guid? convId)
            {
                await _db.ExecuteTransaction(async (conn, tx) =>
                {
                    var db = new Db(conn, tx);
                    await db.SetSavePointAsync();
                    var core = new Service.Service(db);
                    var repl = new EventReplicateWriter(conn, tx);
                    try
                    {
                        var ev = await serv(core);
                        await repl.PutAsync(ev, convId);
                        tx.Commit();
                    }
                    catch (WPayException excp)
                    {
                        await db.RollbackToSavePointAsync();
                        var errEvent = new ErrorRaised(new ErrorValue(excp.Message, excp.Info), convId);
                        await repl.PutAsync(errEvent, convId);
                        tx.Commit();
                    }
                });
            }
        }


    }


}