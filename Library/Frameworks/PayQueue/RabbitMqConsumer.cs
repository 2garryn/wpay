using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Threading.Channels;


namespace wpay.Library.Frameworks.PayQueue
{

    public class RabbitMqConsumer : IQueueConsumer
    {
        private readonly IConnection _conn;
        private readonly Channel<PublishMessage> _poolChannel;

        public RabbitMqConsumer(string user, string pass, string vhost, string hostname)
        {
            ConnectionFactory factory = new ConnectionFactory()
            {
                UserName = user,
                Password = pass,
                VirtualHost = vhost,
                HostName = hostname
            };
            _conn = factory.CreateConnection();
            var options = new BoundedChannelOptions(5)
            {
                FullMode = BoundedChannelFullMode.Wait
            };
            _poolChannel = Channel.CreateBounded<PublishMessage>(options);
            Task.Run(async () => await new ChannelPool(_conn, 5, _poolChannel.Reader).Run());

        }
        public void RegisterCommandConsumer(string queue, IConsumeExecuter executer)
        {
            var channel = _conn.CreateModel();
            channel.ExchangeDeclare(queue, ExchangeType.Direct);
            channel.QueueDeclare(queue, true, false, false, null);
            channel.QueueBind(queue, queue, "", null);
            var consumer = new AsyncEventingBasicConsumer(channel);
            consumer.Received += async (model, ea) =>
                await executer.Execute(GetExchangePublisher(), ea.Body.ToArray());
            channel.BasicConsume(queue, true, consumer);
        }
        public void RegisterEventConsumer(string queue, string[] exchanges, IConsumeExecuter executer)
        {
            var channel = _conn.CreateModel();
            channel.QueueDeclare(queue, true, false, false, null);
            foreach (var exchange in exchanges)
            {
                channel.ExchangeDeclare(queue, ExchangeType.Fanout);
                channel.QueueBind(queue, exchange, "", null);
            }
            var consumer = new AsyncEventingBasicConsumer(channel);
            consumer.Received += async (model, ea) =>
                await executer.Execute(GetExchangePublisher(), ea.Body.ToArray());
            channel.BasicConsume(queue, true, consumer);
        }
        public IExchangePublisher GetExchangePublisher()
        {
            return new PooledExchangePublisher(_poolChannel.Writer);
        }
    }

    public class PooledExchangePublisher : IExchangePublisher
    {
        private readonly ChannelWriter<PublishMessage> _channel;
        public PooledExchangePublisher(ChannelWriter<PublishMessage> channel)
        {
            _channel = channel;
        }

        public async Task PublishEvent(string exchange, byte[] data)
        {          
            await _channel.WriteAsync(new PublishMessage
            {
                Properties = (props) => 
                {
                    props.ContentType = "application/json";
                },
                Body = data,
                ExchangeName = exchange,
                ExchangeType = ExchangeType.Fanout,
                RoutingKey = exchange
            });
        }
        public async Task Command(string exchange, byte[] data)
        {
            await _channel.WriteAsync(new PublishMessage
            {
                Properties = (props) => 
                {
                    props.ContentType = "application/json";
                },
                Body = data,
                ExchangeName = exchange,
                ExchangeType = ExchangeType.Direct,
                RoutingKey = exchange
            });
        }
    }

    public class ChannelPool
    {
        private readonly IConnection _connection;
        private readonly int _size;
        private readonly ChannelReader<PublishMessage> _chan;
        public ChannelPool(IConnection connection, int size, ChannelReader<PublishMessage> chan)
        {
            _size = size;
            _connection = connection;
            _chan = chan;
        }

        public async Task Run()
        {
            Task[] tasks = new Task[_size];
            for (var i = 0; i < _size; i++)
            {
                tasks[i] = RunWorker();
            }

            await Task.WhenAll(tasks);
        }
        public async Task RunWorker()
        {
            var exchanges = new HashSet<string>();
            var model = _connection.CreateModel();
            while (await _chan.WaitToReadAsync())
            {
                if (_chan.TryRead(out var tsk))
                {
                    if(exchanges.Contains(tsk.ExchangeName))
                    {
                        var props = model.CreateBasicProperties();
                        tsk.Properties(props);
                        model.BasicPublish(tsk.ExchangeName, tsk.RoutingKey, true, props, tsk.Body);
                        await Task.Yield();
                    } 
                    else 
                    {
                        model.ExchangeDeclare(tsk.ExchangeName, tsk.ExchangeType);
                        await Task.Yield();
                    }
                }
            }
        }
    }


    public struct PublishMessage
    {
        public Action<IBasicProperties> Properties {get;set;}
        public byte[] Body {get;set;}
        public string ExchangeName {get;set;}
        public string ExchangeType {get;set;}
        public string RoutingKey {get;set;}
    }
}