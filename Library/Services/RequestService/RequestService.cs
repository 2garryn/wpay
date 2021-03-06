using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using ResponseService;
using wpay.Library.Frameworks.PayQueue;
using wpay.Library.Frameworks.PayQueue.Publish;
using wpay.Library.Frameworks.PayQueue.RabbitMqConsumer;
using wpay.Library.Services.ResponseService;

namespace wpay.Library.Services.RequestService
{
    public class RequestService
    {
        private readonly string _rabbitHost;
        private readonly string _rabbitUsername;
        private readonly string _rabbitPassword;

        public RequestService(IConfiguration conf)
        {
            var rabbitSection = conf.GetSection("rabbitmq");
            (_rabbitHost, _rabbitUsername, _rabbitPassword) =
                (rabbitSection["host"], rabbitSection["username"], rabbitSection["password"]);
            Console.WriteLine(_rabbitHost);
            Console.WriteLine(_rabbitUsername);
            Console.WriteLine(_rabbitPassword);
        }

        public async Task Execute()
        {
            var consumer = new RabbitMqConsumer(_rabbitUsername, _rabbitPassword, "/pay_queue", _rabbitHost);
            var factory =
                new ImplFactoryDelegate<RequestServiceDefinition, RequestServiceImpl>(() => new RequestServiceImpl());
            var serv = new ServiceWrapper<RequestServiceDefinition, RequestServiceImpl>(consumer, factory);
            serv.Execute();
            await serv.CallAsync(async (reqServ, publisher) =>

                await reqServ.Create(publisher)
            );
            await consumer.WaitPoolFinished();
        }

    }
}