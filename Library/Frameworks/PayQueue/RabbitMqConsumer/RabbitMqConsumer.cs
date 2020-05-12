using System;
using System.Threading.Tasks;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Threading.Channels;

namespace wpay.Library.Frameworks.PayQueue.RabbitMqConsumer
{

    public class RabbitMqConsumer : IQueueConsumer
    {
        private readonly IConnection _conn;
        private readonly Channel<PublishMessage> _poolChannel;
        private const int PUBLISHER_POOL_SIZE = 5;
        private Task _poolTask;

        public RabbitMqConsumer(string user, string pass, string vhost, string hostname)
        {
            ConnectionFactory factory = new ConnectionFactory()
            {
                UserName = user,
                Password = pass,
                VirtualHost = vhost,
                HostName = hostname,
                DispatchConsumersAsync = true
            };
            _conn = factory.CreateConnection();
            var options = new BoundedChannelOptions(PUBLISHER_POOL_SIZE)
            {
                FullMode = BoundedChannelFullMode.Wait
            };
            _poolChannel = Channel.CreateBounded<PublishMessage>(options);
            _poolTask = Task.Run(async () => await new ChannelPool(_conn, PUBLISHER_POOL_SIZE, _poolChannel.Reader).Run());

        }
        public void RegisterCommandConsumer(string queue, IConsumeExecuter executer)
        {
            var channel = _conn.CreateModel();
            channel.ExchangeDeclare(queue, ExchangeType.Direct, true, false);
            channel.QueueDeclare(queue, true, false, false, null);
            channel.QueueBind(queue, queue, "", null);
            var consumer = new AsyncEventingBasicConsumer(channel);
            consumer.Received += async (model, ea) =>
            {
                await executer.Execute(GetExchangePublisher(), ea.Body.ToArray());
                channel.BasicAck(ea.DeliveryTag, false);
            };
            channel.BasicConsume(queue, false, consumer);
        }
        public void RegisterEventConsumer(string queue, string[] exchanges, IConsumeExecuter executer)
        {
            var channel = _conn.CreateModel();
            channel.QueueDeclare(queue, true, false, false, null);
            foreach (var exchange in exchanges)
            {
                channel.ExchangeDeclare(exchange, ExchangeType.Fanout, true, false);
                channel.QueueBind(queue, exchange, "", null);
            }
            var consumer = new AsyncEventingBasicConsumer(channel);
            consumer.Received += async (model, ea) =>
            {
                await executer.Execute(GetExchangePublisher(), ea.Body.ToArray());
                channel.BasicAck(ea.DeliveryTag, false);
            };
            channel.BasicConsume(queue, false, consumer);
        }
        public IExchangePublisher GetExchangePublisher()
        {
            return new PooledExchangePublisher(_poolChannel.Writer);
        }

        public async Task WaitPoolFinished()
        {
            await _poolTask;
        }
    }

}