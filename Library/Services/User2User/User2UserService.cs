using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using MassTransit;

using wpay.Library.Services.Core.Commands;
using wpay.Library.Services.Core.Messages;
using wpay.Library.Services.Core.Models;
using wpay.Library.Models;

namespace wpay.Library.Services.User2User
{

    public class User2UserService
    {

        private readonly string _rabbitHost;
        private readonly string _rabbitUsername;
        private readonly string _rabbitPassword;
        private readonly string _rabbitEndpoint;
        public User2UserService(IConfiguration conf)
        {
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
                    /*
                    ep.Bind(_rabbitEndpoint, s => 
                    {
                        s.RoutingKey = "user2user_source";

                    ep.Bind(_rabbitEndpoint, s => 
                    {
                        s.RoutingKey = "user2user_destination";
                    });
                    */
                    ep.Consumer(() => new MessageConsumer());
                });
            });

            await bus.StartAsync(); // This is important!
            var accId = new AccountId(Models.UniqId.New());
            var create = new CreateAccount(
                accId,
                CurrencyFactory.New("MXN"),
                false
            );
            var convId = Guid.NewGuid();
            Console.WriteLine($"Publish create account with {accId.Value.Value} with convId = {convId} ");
            await bus.Publish(CreateAccountCommand.From(create), context => context.ConversationId = convId);





            //await bus.Publish(new Message { Text = "Hi", MyInt = new Message2 {Text2 = "sometined2", SomeInt = 123} });

          //  Console.WriteLine("Press any key to exit");
           // await Task.Run(() => Console.ReadKey());

           // await bus.StopAsync();
        }

    }


}