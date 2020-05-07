using System;
using System.Threading.Tasks;

namespace ResponseService
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            var t1 = Task.Run(() => Exec("1", 1000));
            var t2 = Task.Run(() => Exec("2", 1000));
            var t3 = Task.Run(() => Exec("3", 2000));
            var tasks = new Task[] {
                t1, t2, t3
            };
            await Task.WhenAll(tasks);
            await Task.WhenAll(tasks);
        }

        public static async Task Exec(string st, int delay)
        {
            await Task.Delay(delay);
            Console.WriteLine(st);
        }
    }


}
