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
