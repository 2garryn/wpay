using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.EnvironmentVariables;

namespace ResponseService
{
    class Program
    {
        public static async Task Main()
        {
            var configuration = new ConfigurationBuilder()
                .AddEnvironmentVariables()
                .Build();

            Console.WriteLine("Start");
            var serv = new Library.Services.ResponseService.ResponseService(configuration);
            await serv.Execute();
            Console.WriteLine("End");
        }
    }


}
