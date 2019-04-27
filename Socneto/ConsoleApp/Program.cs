using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Socneto.Coordinator.Domain;
using Socneto.Coordinator.Domain.Interfaces;
using Socneto.Coordinator.Infrastructure;


namespace Socneto.Coordinator.ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            MainAsync(args).GetAwaiter().GetResult();
        }

        static async Task MainAsync(string[] args)
        {
            var services = Configure();
            var taskService = services.GetService<TaskService>();

            taskService.StartTaskConsumeTasks();

            Console.WriteLine("doing stuff");
            Console.ReadLine();
            await taskService.StopTaskConsumeTaskAsync();
        }
        
        static IServiceProvider Configure()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                ;

            var configuration = builder.Build();
            
            // add the framework services
            var services = new ServiceCollection()
                .AddLogging(r => r.AddConsole().SetMinimumLevel(LogLevel.Information));
            
            
        

            var builtProvider = services.BuildServiceProvider();

            return builtProvider;
        }
    }
    
   
    
}
