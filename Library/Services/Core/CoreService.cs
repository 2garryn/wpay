using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using wpay.Library.Services.Core.Commands;
using wpay.Library.Services.Core;
using wpay.Library.Libs.Db;
using wpay.Library.Services.Core.Models;
using wpay.Library.Services.Core.Messages;
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

            private DbClient _db;
            public MessageConsumer(DbClient db)
            {
                _db = db;
            }

            public async Task Consume(ConsumeContext<CreateAccountCommand> context)
            {

            }
            public async Task Consume(ConsumeContext<CreateTransactionCommand> context)
            {

            }
            public async Task Consume(ConsumeContext<UpdateTransactionCommand> context)
            {

            }
        }


    /*

        public async Task Handle(CreateAccount createAccount, CreateAccountOptions options, Action<string> error, Action<Account> ok)
        {
            await _db.ExecuteTransaction(async (conn, tx) =>
            {
                try
                {
                    var core = new Service.Service(new Db(conn, tx));
                    var acc = await core.CreateAsync(createAccount, options);

                    tx.Commit();

                }
                catch
                {

                }
            });
        }
        */

    }


}