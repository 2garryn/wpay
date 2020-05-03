using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using wpay.Library.Frameworks.PayQueue;
using System.Text.Json;

namespace wpay.RequestService
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            var qc = new QueueConsumer();
            var sw = new ServiceWrapper<RequestServiceDefinition, RequestServiceImpl>(qc, (c) => 
            {
                return new RequestServiceImpl(c);
            }, "somepayprex");
            
            var resp = new ServiceWrapper<RespServDef, RespServImpl>(qc, (c) => 
            {
                return new RespServImpl(c);
            }, "somepayprex");
            
            resp.Execute();
            sw.Execute();

            await sw.GetService().MakeCommand();
            await sw.GetService().MakeEvent();
        }
    }

    public class QueueConsumer: IQueueConsumer
    {
        private  IConsumeExecuter _execs;
        private IConsumeExecuter _ev;

        public void RegisterCommandConsumer(string queue, IConsumeExecuter executer)
        {
            Console.WriteLine($"Register consume command {queue}");
            _execs = executer;
        }
        public void RegisterEventConsumer(string queue, string[] dispatch, IConsumeExecuter executer)
        {
            Console.WriteLine($"Register consume event {queue}");
            _ev = executer;
        } 
        public IExchangePublisher GetExchangePublisher()
        {
            return new ExchangePublisher(_execs, _ev);
        }
    }

    public class ExchangePublisher :IExchangePublisher
    {
        private  IConsumeExecuter _execs;
        private  IConsumeExecuter _ev;

        public ExchangePublisher( IConsumeExecuter execs, IConsumeExecuter ev)
        {
            _execs = execs;
            _ev = ev;
        }

        public async Task PublishEvent(string endpoint, byte[] data)
        {
            await _ev.Execute(new ExchangePublisher(_execs, _ev), data);
        }
        public async Task Command(string endpoint, byte[] data)
        {
            await _execs.Execute(new ExchangePublisher(_execs, _ev), data);
        }
    }


}
