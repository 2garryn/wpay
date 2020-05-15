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
        private readonly Context _context;
        private readonly DbClient _db;

        public CoreServiceImpl(Context context, DbClient db) =>
            (_context, _db) = (context, db);


        public async Task ConsumeCommand(CreateAccountCommand createAccount) =>
            await Exec(async (core) =>
            {
                var acc = await core.CreateAsync(createAccount.To(), (opts) => opts.IngoreOnDuplicate = true);
                return new AccountCreated() { Event = AccountEvent.From(acc) };
            }, _context.ConversationId);

        public async Task ConsumeCommand(CreateTransactionCommand createTransaction) =>
            await Exec(async (core) =>
            {
                var tran = await core.CreateAsync(createTransaction.To(), (opts) => opts.FailOnExist = false);
                return new TransactionCreated() { Event = TransactionEvent.From(tran) };
            }, _context.ConversationId);

        public async Task ConsumeCommand(UpdateTransactionCommand updateTransaction) =>
            await Exec(async (core) =>
            {
                var tran = await core.UpdateAsync(updateTransaction.To(), (opts) => opts.FailOnUpdateDone = false);
                return new TransactionUpdated() { Event = TransactionEvent.From(tran) };
            }, _context.ConversationId);

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