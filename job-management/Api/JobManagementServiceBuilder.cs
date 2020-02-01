using Domain;
using Domain.Abstract;
using Domain.ComponentManagement;
using Domain.EventTracking;
using Domain.JobStorage;
using Domain.Models;
using Domain.Registration;
using Domain.SubmittedJobConfiguration;
using Infrastructure;
using Infrastructure.ComponentManagement;
using Infrastructure.Kafka;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;
using System.Linq;

namespace Api
{
    public class JobManagementServiceBuilder
    {
        public static IWebHostBuilder GetBuilder(string[] args)
        {
            return WebHost.CreateDefaultBuilder(args)
                .Configure(app =>
                {
                    app.UseCors("_myAllowSpecificOrigins");

                    app.UseRouting();
                    app.UseEndpoints(endpoints =>
                    {
                        endpoints.MapControllers();
                    });
                })
                .ConfigureServices(sp => ConfigureServices(sp, args));
        }

        public static void ConfigureServices(IServiceCollection services, string[] args)
        {
            var useFileStorage = args.Contains("--use_file_storage");
            var noKafka = args.Contains("--no_kafka");
            services.AddRazorPages();
            services.AddCors(options =>
            {
                options.AddPolicy("_myAllowSpecificOrigins",
                    builder =>
                    {
                        builder.WithOrigins("*")
                            .AllowAnyHeader()
                            .AllowAnyMethod();
                    });
            });


            services.AddHostedService<RegistrationRequestListenerHostedService>();
            services.AddTransient<RegistrationRequestListener>();

            services.AddHostedService<EventSendingHostedService>();
            services.AddSingleton<EventQueue>();

            services.AddTransient<IRegistrationRequestProcessor, RegistrationRequestProcessor>();
            services.AddTransient<ISubscribedComponentManager, SubscribedComponentManager>();
            services.AddTransient<IComponentConfigUpdateNotifier, ComponentConfigUpdateNotifier>();

            if (useFileStorage)
            {
                services.AddSingleton<IJobStorage, InMemoryJobStorage>();
                services.AddSingleton<IComponentRegistry, InMemoryRegistry>();
            }
            else
            {
                services.AddHttpClient<IJobStorage, JobStorageProxy>();
                services.AddHttpClient<IComponentRegistry, ComponentStorageProxy>();
            }
            if (noKafka)
            {
                services.AddSingleton(typeof(IEventTracker<>), typeof(NullEventTracker<>));
                services.AddSingleton<IMessageBrokerConsumer, MockKafka>();
                services.AddTransient<IMessageBrokerProducer, MockKafka>();
            }
            else
            {
                services.AddSingleton(typeof(IEventTracker<>), typeof(EventTracker<>));
                services.AddTransient<IMessageBrokerConsumer, KafkaConsumer>();
                services.AddTransient<IMessageBrokerProducer, KafkaProducer>();
            }

            services.AddSingleton<IMessageBrokerApi, KafkaApi>();
            
            var builder = new ConfigurationBuilder()
                           .SetBasePath(Directory.GetCurrentDirectory())
                           .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
            var aspNetCoreEnv = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

            if (aspNetCoreEnv == "Development")
            {
                builder.AddJsonFile($"appsettings.Development.json", true, true);
            }

            var configuration = builder.Build();

            var optionRootName = "JobManagementService";
            services.AddOptions<RegistrationRequestOptions>()
                .Bind(configuration.GetSection($"{optionRootName}:RegistrationRequestOptions"))
                .ValidateDataAnnotations();
            services.AddOptions<KafkaOptions>()
                .Bind(configuration.GetSection($"{optionRootName}:KafkaOptions"))
                .ValidateDataAnnotations();

            services.AddOptions<ComponentIdentifiers>()
                .Bind(configuration.GetSection($"{optionRootName}:ComponentIdentifiers"))
                .ValidateDataAnnotations();

            services.AddOptions<SystemMetricsOptions>()
                .Bind(configuration.GetSection($"{optionRootName}:SystemMetricsOptions"))
                .ValidateDataAnnotations();


            services.AddOptions<StorageChannelNames>()
                .Bind(configuration.GetSection($"{optionRootName}:StorageChannelNames"))
                .ValidateDataAnnotations();

            services.AddOptions<ComponentStorageOptions>()
                .Bind(configuration.GetSection($"{optionRootName}:ComponentStorageOptions"))
                .ValidateDataAnnotations();

            services.AddOptions<RegistrationRequestValidationOptions>()
                .Bind(configuration.GetSection($"{optionRootName}:RegistrationRequestValidationOptions"))
                .ValidateDataAnnotations();

            services.AddOptions<JobStorageOptions>()
                .Bind(configuration.GetSection($"{optionRootName}:JobStorageOptions"))
                .ValidateDataAnnotations();

            services.AddOptions<ComponentOptions>()
                            .Bind(configuration.GetSection($"{optionRootName}:ComponentOptions"))
                            .ValidateDataAnnotations();

            services.AddOptions<JobStorageOptions>()
                .Bind(configuration.GetSection($"{optionRootName}:JobStorageOptions"))
                .ValidateDataAnnotations();
        }

    }
}
