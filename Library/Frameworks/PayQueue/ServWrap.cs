using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Immutable;
using System.Text.Json;

namespace wpay.Library.Frameworks.PayQueue
{
    using MessageType = Type;
    using CallbackAction = Func<object, Context, Task>;

    public class ServWrap<T> where T : IServiceDefinition, new()
    {
        private readonly IQueueConsumer _consumer;
        private readonly Func<IServiceImpl<T>> _impl;
        private readonly string _prefix;
        private readonly PublisherFactory _publisherFactory;

        public ServWrap(IQueueConsumer consumer, Func<IServiceImpl<T>> impl, string prefix) =>
            (_consumer, _impl, _prefix, _publisherFactory) = (consumer, impl, prefix, new PublisherFactory());

        public void Prepare()
        {
            var def = new T();
            OnConsumeCommand(def);
            OnConsumeEvent(def);
            OnCommand(def);
            OnPublishEvent(def);
        }

        private void OnConsumeCommand(T def)
        {
            var queue = _prefix + ":" + def.Label() + ":commands";
            var commandConsume = new Dictionary<MessageType, CallbackAction>();
            def.Configure(new ExecuteConfigurator(_impl)
            {
                OnConsumeCommand = (t, clb) => commandConsume.Add(t, clb)
            });
            var executor = new CallbackExecutor(commandConsume.ToImmutableDictionary(), _publisherFactory.ToPublisher);
            _consumer.RegisterCommandConsumer(queue, executor);
        }

        private void OnConsumeEvent(T def)
        {
            var queue = _prefix + ":" + def.Label() + ":events";
            var eventConsume = new Dictionary<MessageType, CallbackAction>();
            var dispatch = new HashSet<string>();
            def.Configure(new ExecuteConfigurator(_impl)
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
            var executor = new CallbackExecutor(eventConsume.ToImmutableDictionary(), _publisherFactory.ToPublisher);
            _consumer.RegisterEventConsumer(queue, dispatch.ToArray(), executor);
        }

        private void OnCommand(T def)
        {
            var commands = new Dictionary<Type, Dictionary<MessageType, string>>();
            def.Configure(new ExecuteConfigurator(_impl)
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
                        commands.Add(t, messages);
                    }
                }
            });
            _publisherFactory.CommandRoutes = commands
                .ToDictionary(k => k.Key, v => v.Value.ToImmutableDictionary())
                .ToImmutableDictionary();
        }

        private void OnPublishEvent(T def)
        {
            var eventPublish = new Dictionary<MessageType, Func<object, string>>();
            var eventRoutePrefix = _prefix + ":" + def.Label() + ":events";
            def.Configure(new ExecuteConfigurator(_impl)
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