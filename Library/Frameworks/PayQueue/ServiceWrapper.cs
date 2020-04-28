using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text.Json;

namespace wpay.Library.Frameworks.PayQueue
{

    public class ServiceWrapper<T> where T: IServiceDefinition, new()
    {
        
        private IQueueConsumer _consumer;
        private Func<IServiceImpl<T>> _impl;
        public ServiceWrapper(IQueueConsumer consumer, Func<IServiceImpl<T>> impl) => 
            (_consumer, _impl) = (consumer, impl);

        public void Verify() 
        {
           new DefinitionValidator<T>().Validate();
           new ConsumeValidator<T>(_impl).Validate();
        }

        public async Task Execute()
        {
            var def = new T();
            _consumer.RegisterInput(def.Label(), async (publisher, data) => {
                
            });
        }



        private async Task ExecuteRequest(IPublisher publisher, byte[] data)
        {
            var datagram = JsonSerializer.Deserialize<Datagram>(data);
            var context = new Context(datagram.ConversationId, publisher);
            var t = Type.GetType(datagram.Type);
            dynamic deser = JsonSerializer.Deserialize(datagram.Message, t);
            var implInstance = _impl();
            

        }

    }


    public class Configurator : IConfigurator
    {
        public void InputConsume<T>()
        {

        }
        public void InputErrorPublish(string postfix)
        {

        }

        public void EventConsume<S, T>() where S: IServiceDefinition, new()
        {

        }
        public void EventConsume<S, T>(string key) where S: IServiceDefinition, new()
        {

        }
        public void InputSend<S, T>() where S: IServiceDefinition, new()
        {

        }
        public void EventPublish<T>()
        {

        }
        public void EventPublish<T>(Func<T, string> routeFormatter)
        {

        }
    }


}