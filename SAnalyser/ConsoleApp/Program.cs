using System;
using System.IO;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;
using Confluent.Kafka;
using Domain;
using Domain.Interfaces;
using Domain.Models;
using Infrastructure;
using Infrastructure.Kafka;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;


namespace ConsoleApp
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
            
            
            services.AddTransient<TaskService>();

            //services.AddTransient<IConsumer, KafkaConsumer>();
            services.AddTransient<IProducer, KafkaProducer>();

            services.AddTransient<IConsumer, ConsoleConsumer>();
            //services.AddTransient<IProducer, ConsoleProducer>();

            services.AddTransient<IDataCollector, CsvDataCollector>();

            services.Configure<TaskOptions>(configuration.GetSection("Coordinator:TaskOptions"));

            var builtProvider = services.BuildServiceProvider();

            return builtProvider;
        }
    }

    class ConsoleConsumer:IConsumer
    {
        public async Task ConsumeAsync(string topic, Func<string, Task> onRecieveAction, CancellationToken cancellationToken)
        {
            Console.WriteLine("Consuming topic");
            var line = Console.ReadLine();
            await onRecieveAction(line);

        }
    }

    class ConsoleProducer : IProducer
    {
        public Task ProduceAsync(string topicName, Domain.Models.Message message)
        {
            Console.WriteLine("Printing produced message");
            Console.WriteLine(message.Value);
            return Task.CompletedTask;
        }
    }
    
}
