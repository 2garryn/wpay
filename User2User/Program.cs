using System;
using Microsoft.Extensions.Configuration;
using wpay.Library.Services.User2User;
using System.Threading.Tasks;
using System.Text.Json;
using System.Text;
using System.Text.Json.Serialization;
using System.Runtime.Serialization;

namespace User2User
{
    class Program
    {
        public static void Main()
        {
            /*
            var configuration = new ConfigurationBuilder()
                .AddEnvironmentVariables()
                .Build();
            Console.WriteLine("Start");
            var serv = new User2UserService(configuration);
            await serv.Execute();
            Console.WriteLine("End");
            */
            Go();
        }



        public static void Go()
        {
            var msg = new Msg3 { Id = Guid.NewGuid() };
            var name = msg.GetType().FullName;
            var serialized = JsonSerializer.Serialize(msg);
            var t = Type.GetType(name);
            dynamic deser = JsonSerializer.Deserialize(serialized, t);
            var cons = new Consumer();
            cons.Consume(deser);



            dynamic msg4 = new Msg4 { Id = Guid.NewGuid() };

            var genericBase = typeof(IDoer<>);
            var hah = (IDoer<>) cons;
            /*
            var combinedType = genericBase.MakeGenericType(msg4.GetType());
             Console.WriteLine($"Combined {combinedType}");





            dynamic blah = Convert.ChangeType(cons, combinedType);
            var l = blah.GetType() == combinedType;

            Console.WriteLine(cons.ToString());
            foreach (Type imp in cons.GetType().GetInterfaces())
            {
                Console.WriteLine(imp);
            }
            */
        //    Console.WriteLine($"DOOO {l}");


        }

    }






    public class Msg1
    {
        public int Id { get; set; }
    }
    public class Msg2
    {
        public string Value { get; set; }
    }
    public class Msg3
    {
        public Guid Id { get; set; }
    }
    public class Msg4
    {
        public Guid Id { get; set; }
    }


    public interface IConsumer<T>
    {
        void Consume(T t);
    }

    public interface IDoer<T>
    {
        void Do(T t);
    }


    public class Consumer :
        IConsumer<Msg1>,
        IConsumer<Msg2>,
        IConsumer<Msg3>,
        IDoer<Msg4>

    {
        public void Consume(Msg1 msg)
        {
            Console.WriteLine("MSG1");
        }
        public void Consume(Msg2 msg)
        {
            Console.WriteLine(msg.Value);
        }
        public void Consume(Msg3 msg)
        {
            Console.WriteLine("MSG3");
        }

        public void Do(Msg4 msg)
        {
            Console.WriteLine("MSG4");
        }
    }

}
