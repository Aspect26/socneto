using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Kafka;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;

namespace Tests.Integration
{
    class Program
    {
        static async Task Main(string[] args)
        {
            foreach (var ar in Environment.GetCommandLineArgs())
            {
                Console.WriteLine(ar);
            }

            //var kafka = "localhost:9094";
            //kafka = "172.19.0.6:9092";
            var kafka = args[0];
            var kafkaOptions = Options.Create(new KafkaOptions
            {
                ServerAddress = kafka
            });
            if (args[1] == "listen")
            {
                var consumer = new KafkaConsumer(kafkaOptions);

                await consumer.ConsumeAsync(args[2], (message) =>
                {
                    Console.WriteLine(message);
                    return Task.CompletedTask;
                }, CancellationToken.None);
            }
            if (args[1] == "produce")
            {
                var consumer = new KafkaProducer(kafkaOptions);

                await consumer.ProduceAsync(args[2], new Confluent.Kafka.Message<string, string>()
                {
                    Key = "key",
                    Value= args[3]
                });
            }
        }

        //public static IServiceProvider ConfiguraServices()
        //{
        //    var builder = new ConfigurationBuilder()
        //                   .SetBasePath(Directory.GetCurrentDirectory())
        //                   .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
        //    var aspNetCoreEnv = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
        //    if (aspNetCoreEnv == "Development")
        //    {
        //        builder.AddJsonFile($"appsettings.Development.json", true, true);
        //    }
        //    var configuration = builder.Build();

        //    var services = new ServiceCollection();

        //    // TODO take appsettings.json loggin into account.
        //    services.AddLogging(
        //        logging => logging
        //        .AddConsole()
        //        .AddConfiguration(configuration.GetSection("Logging"))
        //        //.SetMinimumLevel(LogLevel.Information)
        //        );


        //    services.AddHttpClient<IJobStorage, JobStorageProxy>();
        //    services.AddTransient<IRegistrationRequestProcessor, RegistrationRequestProcessor>();
        //    services.AddTransient<ISubscribedComponentManager, SubscribedComponentManager>();
        //    services.AddTransient<IComponentConfigUpdateNotifier, ComponentConfigUpdateNotifier>();
        //    services.AddSingleton<StorageApiTester>();

        //    services.AddSingleton<IMessageBrokerConsumer, MockKafka>();
        //    services.AddTransient<IMessageBrokerProducer, MockKafka>();

        //    //services.AddTransient<IMessageBrokerConsumer, KafkaConsumer>();
        //    //services.AddTransient<IMessageBrokerProducer, KafkaProducer>();

        //    services.AddHttpClient<IComponentRegistry, ComponentStorageProxy>();
        //    services.AddSingleton<IMessageBrokerApi, KafkaApi>();

        //    var optionRootName = "JobManagementService";
        //    services.AddOptions<RegistrationRequestOptions>()
        //        .Bind(configuration.GetSection($"{optionRootName}:RegistrationRequestOptions"))
        //        .ValidateDataAnnotations();
        //    services.AddOptions<KafkaOptions>()
        //        .Bind(configuration.GetSection($"{optionRootName}:KafkaOptions"))
        //        .ValidateDataAnnotations();

        //    services.AddOptions<ComponentIdentifiers>()
        //        .Bind(configuration.GetSection($"{optionRootName}:ComponentIdentifiers"))
        //        .ValidateDataAnnotations();

        //    services.AddOptions<StorageChannelNames>()
        //        .Bind(configuration.GetSection($"{optionRootName}:StorageChannelNames"))
        //        .ValidateDataAnnotations();

        //    services.AddOptions<ComponentStorageOptions>()
        //        .Bind(configuration.GetSection($"{optionRootName}:ComponentStorageOptions"))
        //        .ValidateDataAnnotations();

        //    services.AddOptions<RegistrationRequestValidationOptions>()
        //        .Bind(configuration.GetSection($"{optionRootName}:RegistrationRequestValidationOptions"))
        //        .ValidateDataAnnotations();

        //    services.AddOptions<JobStorageOptions>()
        //        .Bind(configuration.GetSection($"{optionRootName}:JobStorageOptions"))
        //        .ValidateDataAnnotations();

        //    services.AddOptions<JobStorageOptions>()
        //        .Bind(configuration.GetSection($"{optionRootName}:JobStorageOptions"))
        //        .ValidateDataAnnotations();


        //    return services.BuildServiceProvider();
        //}
    }



}
