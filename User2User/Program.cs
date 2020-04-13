using System;
using Microsoft.Extensions.Configuration;
using wpay.Library.Services.User2User;
using System.Threading.Tasks;

namespace User2User
{
    class Program
    {
        public static async Task Main()
        {
            var configuration = new ConfigurationBuilder()
                .AddEnvironmentVariables()
                .Build();
            Console.WriteLine("Start");
            var serv = new User2UserService(configuration);
            await serv.Execute();
            Console.WriteLine("End");
        }
    }
}
