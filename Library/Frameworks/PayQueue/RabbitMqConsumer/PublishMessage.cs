using System;
using RabbitMQ.Client;

namespace wpay.Library.Frameworks.PayQueue.RabbitMqConsumer
{
    public struct PublishMessage
    {
        public Action<IBasicProperties> Properties {get;set;}
        public byte[] Body {get;set;}
        public string ExchangeName {get;set;}
        public string ExchangeType {get;set;}
        public string RoutingKey {get;set;}
        public string QueueName {get;set;}
    }
}