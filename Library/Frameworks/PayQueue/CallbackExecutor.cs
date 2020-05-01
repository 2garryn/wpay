using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections.Immutable;
using System.Threading.Tasks;
using System.Text.Json;

namespace wpay.Library.Frameworks.PayQueue
{
    using CallbackAction = Func<object, Context, Task>;
    using MessageType = Type;
    using ToPublisher = Func<IExchangePublisher, Datagram, Publisher>;
    public class CallbackExecutor : IConsumeExecuter
    {


        private readonly ImmutableDictionary<MessageType, CallbackAction> _consumers;
        private readonly ToPublisher _toPublisher;
        public CallbackExecutor(ImmutableDictionary<MessageType, CallbackAction> consumers, ToPublisher toPublisher)
        {
            _consumers = consumers;
            _toPublisher = toPublisher;
        }

        public async Task Execute(IExchangePublisher exchangePublisher, byte[] data)
        {
            var datagram = JsonSerializer.Deserialize<Datagram>(data);
            var publisher = _toPublisher(exchangePublisher, datagram);
            var context = new Context(datagram.ConversationId, publisher);
            var t = Type.GetType(datagram.Type);
            object deser = JsonSerializer.Deserialize(datagram.Message, t);
            var executor = _consumers.GetValueOrDefault(t, null!);
            await executor(deser, context);
        }

    }


}