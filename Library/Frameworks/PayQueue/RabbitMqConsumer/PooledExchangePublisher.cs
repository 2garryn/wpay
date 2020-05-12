using System;
using System.Threading.Tasks;
using RabbitMQ.Client;
using System.Threading.Channels;

namespace wpay.Library.Frameworks.PayQueue.RabbitMqConsumer
{

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
                RoutingKey = ""
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
                RoutingKey = ""
            });
        }
    }


}