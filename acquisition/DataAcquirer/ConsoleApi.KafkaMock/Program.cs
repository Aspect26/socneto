using Domain.Abstract;
using Infrastructure.Kafka;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Threading.Tasks;

namespace ConsoleApi.KafkaMock
{
    static class Program
    {
        public static void Main(string[] args)
        {
            MainAsync(args).GetAwaiter().GetResult();            
        }

        public static async Task MainAsync(string[] args)
        {
#if DEBUG
            args = new[]
            {
                "commands.json",
                //"job_management.job_configuration.DataAcquirer_Twitter",
                "job_management.job_configuration.DataAcquirer_StaticDataSet_movietweets",
                Path.Combine(Directory.GetCurrentDirectory(),"output_data")
            };
#endif

            var services = Configure();
            var app = services.GetRequiredService<App>();
            var commandFilePath = args[0];
            var configTopic = args[1];
            var outputDirectory = new DirectoryInfo(args[2]);
            try
            {
                //outputDirectory.Create();
            }
            catch (IOException)
            {
                // intentionally empty
            }
            await app.DoAsync(commandFilePath, configTopic, outputDirectory);
        }

        public static IServiceProvider Configure()
        {
            var builder = new ConfigurationBuilder()
                           .SetBasePath(Directory.GetCurrentDirectory())
                           .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
            var aspNetCoreEnv = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            if (aspNetCoreEnv == "Development")
            {
                builder.AddJsonFile($"appsettings.Development.json", true, true);
            }
            var configuration = builder.Build();

            var services = new ServiceCollection();

            // TODO take appsettings.json loggin into account.
            services.AddLogging(
                logging => logging
                .AddConsole()
                .AddConfiguration(configuration.GetSection("Logging"))                
                //.SetMinimumLevel(LogLevel.Information)
                );


            services.AddSingleton<IMessageBrokerProducer, KafkaProducer>();

            services.AddSingleton<IMessageBrokerConsumer, KafkaConsumer>();

            services.AddSingleton<App>();
            services.AddSingleton<PostSaver>();
            services.AddSingleton<CommandFileReader>();
            services.AddSingleton<CommandSender>();

            services.AddOptions<KafkaOptions>()
                .Bind(configuration.GetSection($"KafkaOptions"))
                .ValidateDataAnnotations();

            return services.BuildServiceProvider();
        }

    }
}
