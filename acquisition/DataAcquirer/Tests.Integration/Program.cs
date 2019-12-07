using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Domain;
using Domain.Abstract;
using Domain.JobManagement;
using Domain.Registration;
using Infrastructure;
using Infrastructure.Kafka;
using Infrastructure.Metadata;
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
            var services = ConfiguraServices();

            var storageTest = services.GetRequiredService<StorageApiTester>();

            await storageTest.TestAsync();

        }

        public static IServiceProvider ConfiguraServices()
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
            services.AddHttpClient();
            // TODO take appsettings.json loggin into account.
            services.AddLogging(
                logging => logging
                .AddConsole()
                .AddConfiguration(configuration.GetSection("Logging"))
                //.SetMinimumLevel(LogLevel.Information)
                );


            services.AddSingleton<IDataAcquirerMetadataStorage,MetadataStorageProxy>();
            services.AddSingleton<StorageApiTester>();

            services.AddOptions<MetadataStorageProxyOptions>()
                .Bind(configuration.GetSection("DataAcquisitionService:MetadataStorageProxyOptions"))
                .ValidateDataAnnotations();

            services.AddOptions<ComponentOptions>()
                .Bind(configuration.GetSection("DataAcquisitionService:ComponentOptions"))
                .ValidateDataAnnotations();

            return services.BuildServiceProvider();
        }
    }

    

}
