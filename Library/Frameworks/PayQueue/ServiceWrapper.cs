using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using System.Text.Json;
using System.Text;

namespace wpay.Library.Frameworks.PayQueue
{
    using CallbackAction = Func<object, Context, Task>;
    public class ServiceWrapper<T> where T : IServiceDefinition, new()
    {
        private IQueueConsumer _consumer;
        private Func<IServiceImpl<T>> _impl;
        private ImmutableDictionary<Type, Dictionary<Type, string>> _sendInputs;
        private ImmutableDictionary<Type, Func<object, string>> _publishEventsRouted;
        private ImmutableHashSet<Type> _publishEvents;
        private string _label;

        public ServiceWrapper(IQueueConsumer consumer, Func<IServiceImpl<T>> impl) =>
            (_consumer, _impl) = (consumer, impl);

        public void Validate()
        {
            new DefinitionValidator<T>().Validate();
            new ConsumeValidator<T>(_impl).Validate();
        }
        public void Prepare()
        {
            var conf = new Configurator(_impl);
            var def = new T();
            _label = def.Label();
            def.Configure(conf);

            var consumers = conf.Inputs.ToImmutableDictionary();

            _consumer.RegisterInputConsumer(_label + ":input", async (publisher, data) =>
                await CallExecutor(consumers, publisher, data));

            var eventConsumers = conf.Events.ToImmutableDictionary();

            _consumer.RegisterEventConsumer(_label + ":events", conf.EventsProducers.ToArray(), async (publisher, data) =>
                await CallExecutor(eventConsumers, publisher, data));

            _sendInputs = conf.SendInputs.ToImmutableDictionary();
            _publishEventsRouted = conf.PublishEventsRouted.ToImmutableDictionary();
            _publishEvents = conf.PublishEvents.ToImmutableHashSet();
        }


        private async Task CallExecutor(ImmutableDictionary<Type, CallbackAction> consumers, IExchangePublisher exchangePublisher, byte[] data)
        {
            var datagram = JsonSerializer.Deserialize<Datagram>(data);
            var publisher = new Publisher(_label, datagram.ConversationId)
            {
                ExchangePublisher = exchangePublisher,
                SendInputs = _sendInputs,
                PublishEvents = _publishEvents,
                PublishEventsRouted = _publishEventsRouted
            };

            var context = new Context(datagram.ConversationId, publisher);
            var t = Type.GetType(datagram.Type);
            object deser = JsonSerializer.Deserialize(datagram.Message, t);
            var executor = consumers.GetValueOrDefault(t, null!);
            await executor(deser, context);
        }
    }

    public class Publisher
    {
        internal IExchangePublisher ExchangePublisher {get; set;}
        internal ImmutableDictionary<Type, Dictionary<Type, string>> SendInputs {get;set;}
        internal ImmutableDictionary<Type, Func<object, string>> PublishEventsRouted {get;set;}
        internal ImmutableHashSet<Type> PublishEvents {get;set;}

        private Guid _conversationId;
        private string _label;

        internal Publisher(string label, Guid conversationId) => 
            (_label, _conversationId) = (label, conversationId);

        public async Task Send<S, T>(T message) where S : IServiceDefinition, new() 
        { 
            var exch = SendInputs[typeof(S)][typeof(T)];
            var serMessage = JsonSerializer.Serialize(message);
            var datagram = new Datagram
            {
                ConversationId = _conversationId,
                Type = typeof(T).FullName,
                Message = Encoding.UTF8.GetBytes(serMessage)
            };
            var bin = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(datagram));
            await ExchangePublisher.PublishInput(exch, bin);
        }

        public async Task Publish<T>(T message) 
        { 
            var mType = typeof(T);
            if (!PublishEvents.Contains(mType) && !PublishEventsRouted.ContainsKey(mType))
            {
                throw new Exception($"Type was not registered {typeof(T)}");
            }
            var serMessage = JsonSerializer.Serialize(message);
            var datagram = new Datagram
            {
                ConversationId = _conversationId,
                Type = typeof(T).FullName,
                Message = Encoding.UTF8.GetBytes(serMessage)
            };
            var bin = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(datagram));
            if(PublishEvents.Contains(mType))
            {
                await ExchangePublisher.PublishEvent(_label + ":events", bin);
                return;
            };
            var route = PublishEventsRouted[mType](message!);
            await ExchangePublisher.PublishEvent(_label + ":events:" + route, bin);
        }

    }


    public class Configurator : IConfigurator
    {
        private readonly Func<object> _servCreator;
        public Dictionary<Type, CallbackAction> Inputs { get; }
        public Dictionary<Type, CallbackAction> Events { get; }
        public List<string> EventsProducers { get; }
        
        public Dictionary<Type, Dictionary<Type, string>> SendInputs { get; }
        public Dictionary<Type, Func<object, string>> PublishEventsRouted { get; }
        public HashSet<Type> PublishEvents {get;}

        public Configurator(Func<object> servCreator)
        {
            _servCreator = servCreator;
            Inputs = new Dictionary<Type, CallbackAction>();
            Events = new Dictionary<Type, CallbackAction>();
            EventsProducers = new List<string>() { };
            SendInputs = new Dictionary<Type, Dictionary<Type, string>>();
            PublishEventsRouted = new Dictionary<Type, Func<object, string>>();
            PublishEvents = new HashSet<Type>();
        }

        public void InputConsume<T>() =>
            Inputs[typeof(T)] = async (object val, Context context) =>
                await ((IInputConsumer<T>)_servCreator()).InputConsume((T)val, context);

        public void EventConsume<S, T>() where S : IServiceDefinition, new()
        {
            EventsProducers.Add(new S().Label());
            Events[typeof(T)] = async (object val, Context context) =>
                await ((IEventConsumer<S, T>)_servCreator()).EventConsume((T)val, context);
        }

        public void EventConsume<S, T>(string key) where S : IServiceDefinition, new()
        {
            EventsProducers.Add(new S().Label() + ":events:" + key);
            Events[typeof(T)] = async (object val, Context context) =>
                await ((IEventConsumer<S, T>)_servCreator()).EventConsume((T)val, context);
        }

        public void InputErrorPublish(string postfix) { }

        public void InputSend<S, T>() where S : IServiceDefinition, new()
        {
            var path = new S().Label() + ":input";
            var sType = typeof(S);
            var mType = typeof(T);

            Dictionary<Type, string> message;
            if (SendInputs.TryGetValue(sType, out message))
            {
                message.Add(mType, path);
            }
            else
            {
                message = new Dictionary<Type, string>()
                {
                    [mType] = path
                };
                SendInputs.Add(sType, message);
            }
        }

        public void EventPublish<T>() 
        {
            if (!PublishEvents.Add(typeof(T)))
            {
                throw new Exception($"Type is already defined: {typeof(T)}");
            }
        }
        public void EventPublish<T>(Func<T, string> routeFormatter) =>
            PublishEventsRouted.Add(typeof(T), (object o) => routeFormatter((T) o));
        
    }


}