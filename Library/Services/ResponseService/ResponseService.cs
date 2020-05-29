using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using ResponseService;
using wpay.Library.Frameworks.PayQueue;
using wpay.Library.Frameworks.PayQueue.Consume;
using wpay.Library.Frameworks.PayQueue.RabbitMqConsumer;
using wpay.Library.Services.ResponseService;

namespace Library.Services.ResponseService
{
    public class ResponseService
    {
        private readonly string _rabbitHost;
        private readonly string _rabbitUsername;
        private readonly string _rabbitPassword;

        public ResponseService(IConfiguration conf)
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
                new ImplFactoryDelegate<ResponseServiceDefinition, ResponseServiceImpl>(() => new ResponseServiceImpl());
            var serv = new ServiceWrapper<ResponseServiceDefinition, ResponseServiceImpl>(consumer,  factory,
                (conf) =>
                {
                    conf.UseErrorEventHandling(() => new ErrorHandler());
                    conf.UseErrorCommandHandling(() => new ErrorHandler());
                });
            serv.Execute();
            await consumer.WaitPoolFinished();
        }
    }
}