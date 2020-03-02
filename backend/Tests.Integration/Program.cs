using System;
using System.IO;
using System.Threading.Tasks;
using Domain.EventTracking;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Socneto.Domain;
using Socneto.Domain.EventTracking;
using Socneto.Domain.Services;
using Socneto.Infrastructure.Kafka;

namespace Tests.Integration
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var services = ConfigureServices();
            
            var storageTest = services.GetRequiredService<StorageApiTester>();
            await storageTest.TestAsync();
            
            // TODO: add jms service
        }

        public static IServiceProvider ConfigureServices()
        {
            var builder = new ConfigurationBuilder()
                           .SetBasePath(Directory.GetCurrentDirectory())
                           .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
            var configuration = builder.Build();

            var services = new ServiceCollection();

            // TODO take appsettings.json loggin into account.
            services.AddLogging(
                logging => logging
                .AddConsole()
                .AddConfiguration(configuration.GetSection("Logging"))
                //.SetMinimumLevel(LogLevel.Information)
                );


            services.AddSingleton<StorageApiTester>();

            services.AddSingleton<IResultProducer, MockResultProducer>();
            services.AddTransient<IMessageBrokerProducer, MockKafka>();
            services.AddSingleton(typeof(IEventTracker<>), typeof(ConsoleEventTracker<>));
            
            services.AddTransient<IStorageService, StorageService>();
            
            var optionRootName = "Socneto";

            services.AddOptions<KafkaOptions>()
                .Bind(configuration.GetSection($"{optionRootName}:KafkaOptions"))
                .ValidateDataAnnotations();

            services.AddOptions<JMSOptions>()
                .Bind(configuration.GetSection($"{optionRootName}:JobManagementServiceOptions"))
                .ValidateDataAnnotations();

//            services.AddOptions<StorageOptions>()
//                .Bind(configuration.GetSection($"{optionRootName}:StorageOptions"))
//                .ValidateDataAnnotations();

            services.Configure<StorageOptions>(configuration.GetSection("Socneto:StorageOptions"));

            services.AddOptions<DefaultAcquirersCredentialsOptions>()
                .Bind(configuration.GetSection($"{optionRootName}:DefaultAcquirersCredentials"))
                .ValidateDataAnnotations();

            return services.BuildServiceProvider();
        }
    }

    

}