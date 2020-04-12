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

            public async Task Consume(ConsumeContext<CreateAccountCommand> context)
            {
                var create = context.Message.To();
                var options = new CreateAccountOptions()
                {
                    IngoreOnDuplicate = true
                };
                await _db.ExecuteTransaction(async (conn, tx) =>
                {
                    var db = new Db(conn, tx);
                    var core = new Service.Service(db);
                    var acc = await core.CreateAsync(create, options);
                    var repl = new EventReplicator(conn, tx);
                    await repl.PutAsync(new AccountCreated(acc, context.ConversationId));
                    tx.Commit();
                });
            }
            public async Task Consume(ConsumeContext<CreateTransactionCommand> context)
            {
                var create = context.Message.To();
                var options = new CreateTransactionOptions()
                {
                    FailOnExist = false
                };
                await _db.ExecuteTransaction(async (conn, tx) =>
                {
                    var db = new Db(conn, tx);
                    await db.SetSavePointAsync();
                    var core = new Service.Service(db);
                    var repl = new EventReplicator(conn, tx);
                    try
                    {
                        var tran = await core.CreateAsync(create, options);
                        await repl.PutAsync(new TransactionCreated(tran, context.ConversationId));
                        tx.Commit();
                    }
                    catch(WPayException excp)
                    {
                        await db.RollbackToSavePointAsync();
                        var errEvent = new ErrorRaised(excp.Message, excp.Info);
                        await repl.PutAsync(errEvent);
                        tx.Commit();
                    }
                });
            }
            public async Task Consume(ConsumeContext<UpdateTransactionCommand> context)
            {
                var update = context.Message.To();
                var options = new UpdateTransactionOptions()
                {
                    FailOnUpdateDone = false
                };
                await _db.ExecuteTransaction(async (conn, tx) =>
                {
                    var db = new Db(conn, tx);
                    await db.SetSavePointAsync();
                    var core = new Service.Service(db);
                    var repl = new EventReplicator(conn, tx);
                    try
                    {
                        var tran = await core.UpdateAsync(update, options);
                        await repl.PutAsync(new TransactionUpdated(tran, context.ConversationId));
                        tx.Commit();
                    }
                    catch(WPayException excp)
                    {
                        await db.RollbackToSavePointAsync();
                        var errEvent = new ErrorRaised(excp.Message, excp.Info);
                        await repl.PutAsync(errEvent);
                        tx.Commit();
                    }
                });
            }
        }


    }


}