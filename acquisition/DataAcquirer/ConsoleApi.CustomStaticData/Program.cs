using System;

using Domain;
using Domain.Abstract;
using Domain.Acquisition;
using Domain.JobConfiguration;
using Domain.JobManagement;
using Domain.JobManagement.Abstract;
using Domain.Registration;
using Infrastructure.DataGenerator;
using Infrastructure.Kafka;
using Infrastructure.Reddit;
using Infrastructure.CustomStaticData;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.IO;


namespace ConsoleApi.CustomStaticData
{
    class Program
    {
        static void Main()
        {
            var services = Configure();
            var app = services.GetRequiredService<CustomStaticDataApp>();

            app.DoAsync().GetAwaiter().GetResult();
        }

        public static IServiceProvider Configure()
        {
            var builder = new ConfigurationBuilder()
                           .SetBasePath(Directory.GetCurrentDirectory())
                           .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
#if DEBUG
            builder.AddJsonFile($"appsettings.Development.json", true, true);
#endif
            var configuration = builder.Build();

            var services = new ServiceCollection();
            services.AddLogging(
                logging => logging
                .AddConsole()
                .SetMinimumLevel(LogLevel.Information));

            services.AddSingleton<JobConfigurationUpdateListener>();
            services.AddHostedService<JobConfigurationUpdateListenerHostedService>();

            services.AddTransient<IJobManager, JobManager>();

            services.AddTransient<IRegistrationService, RegistrationService>();



            services.AddSingleton<RedditContextProvider>();

            services.AddSingleton<JobConfigurationUpdateListenerHostedService>();

            services.AddTransient<IMessageBrokerProducer, KafkaProducer>();
            services.AddSingleton<IMessageBrokerConsumer, MockConsumer>();


            services.AddSingleton<IDataAcquirerJobStorage, DataAcquirerJobFileStorage>();
            services.AddSingleton(typeof(IEventTracker<>), typeof(NullEventTracker<>));

            services.AddTransient<IDataAcquirer, CustomStaticDataAcquirer>();

            services.AddSingleton<CustomStaticDataApp>();
            services.AddSingleton<CustomStreamReaderFactory>();

            var rootName = "DataAcquisitionService";

            services.AddOptions<ComponentOptions>()
                .Bind(configuration.GetSection($"{rootName}:ComponentOptions"))
                .ValidateDataAnnotations();

            services.AddOptions<RegistrationRequestOptions>()
                .Bind(configuration.GetSection($"{rootName}:RegistrationRequestOptions"))
                .ValidateDataAnnotations();

            services.AddOptions<MinioOptions>()
                .Bind(configuration.GetSection($"{rootName}:MinioOptions"))
                .ValidateDataAnnotations();


            services.AddOptions<AttributeElementNames>()
             .Bind(configuration.GetSection($"{rootName}:AttributeElementNames"))
             .ValidateDataAnnotations();



            services.AddOptions<KafkaOptions>()
                .Bind(configuration.GetSection($"{rootName}:KafkaOptions"))
                .ValidateDataAnnotations();

            return services.BuildServiceProvider();

        }

    }
}
