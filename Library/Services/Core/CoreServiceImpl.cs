using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using wpay.Library.Frameworks.PayQueue;
using wpay.Library.Libs.Db;
using wpay.Library.Services.Core.Outbox;
using wpay.Library.Services.Core.Commands;
using wpay.Library.Models;
using wpay.Library.Services.Core.Models;
using wpay.Library.Services.Core.Definition;

namespace wpay.Library.Services.Core
{

    public class CoreServiceImpl :
        IServiceImpl<CoreServiceDefinition>,
        ICommandConsumer<CreateAccountCommand>,
        ICommandConsumer<CreateTransactionCommand>,
        ICommandConsumer<UpdateTransactionCommand>
    {
        private readonly DbClient _db;

        public CoreServiceImpl(DbClient db) =>
            ( _db) = ( db);


        public async Task ConsumeCommand(MessageContext<CreateAccountCommand> createAccount) =>
            await Exec(async (core) =>
            {
                var acc = await core.CreateAsync(createAccount.Message.To(), (opts) => opts.IngoreOnDuplicate = true);
                return new AccountCreated() { Event = AccountEvent.From(acc) };
            }, createAccount.ConversationId);

        public async Task ConsumeCommand(MessageContext<CreateTransactionCommand> createTransaction) =>
            await Exec(async (core) =>
            {
                var tran = await core.CreateAsync(createTransaction.Message.To(), (opts) => opts.FailOnExist = false);
                return new TransactionCreated() { Event = TransactionEvent.From(tran) };
            }, createTransaction.ConversationId);

        public async Task ConsumeCommand(MessageContext<UpdateTransactionCommand> updateTransaction) =>
            await Exec(async (core) =>
            {
                var tran = await core.UpdateAsync(updateTransaction.Message.To(), (opts) => opts.FailOnUpdateDone = false);
                return new TransactionUpdated() { Event = TransactionEvent.From(tran) };
            }, updateTransaction.ConversationId);

        private async Task Exec(Func<Service.Service, Task<ICoreEvent>> serv, Guid? convId)
        {
            await _db.ExecuteTransaction(async (conn, tx) =>
            {
                /*
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
                    await repl.PutAsync(new ErrorRaised { Error = excp.Message, Info = excp.Info }, convId);
                    tx.Commit();
                }
                */
            });
        }

    }



}