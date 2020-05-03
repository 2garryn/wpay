using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Immutable;


namespace wpay.Library.Frameworks.PayQueue
{
    using MessageType = Type;
    using CallbackAction = Func<object, Context, Task>;

    public delegate TImpl ImplFactory<TImpl, TServDef>(Context context)
            where TServDef : IServiceDefinition, new()
            where TImpl : IServiceImpl<TServDef>;


    public class ServiceWrapper<TServDef, TImpl>
        where TServDef : IServiceDefinition, new()
        where TImpl : IServiceImpl<TServDef>
    {
        private readonly IQueueConsumer _consumer;
        private readonly ImplFactory<TImpl, TServDef> _impl;
        private readonly string _prefix;
        private readonly PublisherFactory _publisherFactory;

        public ServiceWrapper(IQueueConsumer consumer, ImplFactory<TImpl, TServDef> impl, string prefix) =>
            (_consumer, _impl, _prefix, _publisherFactory) = (consumer, impl, prefix, new PublisherFactory());

        public void Execute()
        {
            var def = new TServDef();
            OnConsumeCommand(def);
            OnConsumeEvent(def);
            OnCommand(def);
            OnPublishEvent(def);
        }

        public TImpl GetService() =>
            GetService((ICallParameters p) =>
            {
                p.ConversationId = null;
            });

        public TImpl GetService(Action<ICallParameters> p)
        {
            var callParameters = new CallParameters();
            p(callParameters);

            var context = new Context(
                Guid.NewGuid(),
                callParameters.ConversationId,
                _publisherFactory.ToPublisher(_consumer.GetExchangePublisher())
            );
            return _impl(context);
        }

        private void OnConsumeCommand(TServDef def)
        {
            var queue = _prefix + ":" + def.Label() + ":commands";
            var commandConsume = new Dictionary<MessageType, CallbackAction>();
            def.Configure(new ExecuteConfigurator((c) => _impl(c))
            {
                OnConsumeCommand = (t, clb) =>
                {

                    commandConsume.Add(t, clb);
                } 
            });
            if (commandConsume.Count() > 0)
            {
                var executor = new CallbackExecutor(commandConsume.ToImmutableDictionary(), _publisherFactory.ToPublisher);
                _consumer.RegisterCommandConsumer(queue, executor);
            }
        }

        private void OnConsumeEvent(TServDef def)
        {
            var queue = _prefix + ":" + def.Label() + ":events";
            var eventConsume = new Dictionary<MessageType, CallbackAction>();
            var dispatch = new HashSet<string>();
            def.Configure(new ExecuteConfigurator((c) => _impl(c))
            {
                OnConsumeEvent = (t, servdef, clb) =>
                {
                    eventConsume.Add(t, clb);
                    dispatch.Add(_prefix + ":" + servdef.Label() + ":events");
                },
                OnConsumeEventRouted = (t, servdef, key, clb) =>
                {
                    eventConsume.Add(t, clb);
                    dispatch.Add(_prefix + ":" + servdef.Label() + ":events:" + key);
                }
            });
            if (eventConsume.Count() > 0)
            {
                var executor = new CallbackExecutor(eventConsume.ToImmutableDictionary(), _publisherFactory.ToPublisher);
                _consumer.RegisterEventConsumer(queue, dispatch.ToArray(), executor);
            }
        }

        private void OnCommand(TServDef def)
        {
            var commands = new Dictionary<Type, Dictionary<MessageType, string>>();
            def.Configure(new ExecuteConfigurator((c) => _impl(c))
            {
                OnCommand = (t, servdef) =>
                {
                    var path = _prefix + ":" + servdef.Label() + ":commands";
                    var servtype = servdef.GetType();
                    Dictionary<MessageType, string> messages;
                    if (commands.TryGetValue(servtype, out messages))
                    {
                        messages.Add(t, path);
                    }
                    else
                    {
                        messages = new Dictionary<MessageType, string>() { [t] = path };
                        commands.Add(servtype, messages);
                    }
                }
            });
            
            _publisherFactory.CommandRoutes = commands
                .ToDictionary(k => k.Key, v => v.Value.ToImmutableDictionary())
                .ToImmutableDictionary();
        }

        private void OnPublishEvent(TServDef def)
        {
            var eventPublish = new Dictionary<MessageType, Func<object, string>>();
            var eventRoutePrefix = _prefix + ":" + def.Label() + ":events";
            def.Configure(new ExecuteConfigurator((c) => _impl(c))
            {
                OnPublishEvent = (t) =>
                    eventPublish.Add(t, (m) => eventRoutePrefix),
                OnPublishEventRouted = (t, formatter) =>
                    eventPublish.Add(t, (m) => eventRoutePrefix + ":" + formatter(m))
            });
            _publisherFactory.EventRoutes = eventPublish.ToImmutableDictionary();
        }
    }
}