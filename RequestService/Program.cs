using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Text.Json;
using Microsoft.Extensions.Configuration;

namespace wpay.RequestService
{
    class Program
    {
        public static async Task Main()
        {
            var configuration = new ConfigurationBuilder()
                .AddEnvironmentVariables()
                .Build();

            Console.WriteLine("Start");
            var serv = new wpay.Library.Services.RequestService.RequestService(configuration);
            await serv.Execute();
            Console.WriteLine("End");
        }
    }


}
