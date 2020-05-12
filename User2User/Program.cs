using System;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;
using System.Text;
using wpay.Library.Services.User2User;
using wpay.Library.Frameworks.PayQueue.RabbitMqConsumer;
using wpay.Library.Frameworks.PayQueue;

namespace User2User
{
    class Program
    {
        public async static Task Main()
        {
            var user = "pi";
            var pwd = "raspberry";
            var vhost = "/";
            var hostname = "192.168.88.19";
            var consumer = new RabbitMqConsumer(user, pwd, vhost, hostname);
            consumer.RegisterCommandConsumer("wpay_command_consumer", new ConsumeExecuter("command"));
            var exchanges = new string[] 
            {
                "wpay_exchange_1",
                "wpay_exchange_2"
            };
            consumer.RegisterEventConsumer("wpay_command_consumer", exchanges, new ConsumeExecuter("event"));
            /*
            await consumer
                .GetExchangePublisher()
                .Command("wpay_command_consumer", Encoding.UTF8.GetBytes("test"));
            
            await consumer
                .GetExchangePublisher()
                .Command("wpay_command_consumer", Encoding.UTF8.GetBytes("message 2"));
            */

            await Task.Delay(300000);
        }

    }

    public class  ConsumeExecuter: IConsumeExecuter
    {
        private string _prefix;
        public ConsumeExecuter(string pref)
        {
            _prefix = pref;
        }
        public async Task Execute(IExchangePublisher exchangePublisher, byte[] data)
        {
            var strData = System.Text.Encoding.UTF8.GetString(data, 0, data.Length);
            Console.WriteLine($"Consumer '{_prefix}' Message received: {strData}");
            Task.Yield();
        }
    }

}
