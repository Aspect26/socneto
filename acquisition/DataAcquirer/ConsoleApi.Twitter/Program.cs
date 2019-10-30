using Application;
using Domain;
using Domain.Abstract;
using Domain.Acquisition;
using Domain.JobConfiguration;
using Domain.JobManagement;
using Domain.Model;
using Domain.Registration;
using Infrastructure.Kafka;
using Infrastructure.Twitter;
using LinqToTwitter;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Domain.JobManagement.Abstract;
using Infrastructure.DataGenerator;
using Infrastructure.StaticData;

namespace ConsoleApi.Twitter
{
    public class Program
    {
        public static void Main(string[] args)
        {
            MainAsync(args).GetAwaiter().GetResult();
        }


        public static IServiceProvider Build()
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
            services.AddLogging(
                logging => logging
                .AddConsole()
                .SetMinimumLevel(LogLevel.Information));
            
            services.AddSingleton<JobConfigurationUpdateListener>();
            services.AddHostedService<JobConfigurationUpdateListenerHostedService>();

            services.AddTransient<IJobManager, JobManager>();

            services.AddTransient<IRegistrationService, RegistrationService>();
            
            services.AddTransient<IDataAcquirer, TwitterDataAcqirer>();

            services.AddSingleton<JobConfigurationUpdateListenerHostedService>();

            services.AddTransient<IMessageBrokerProducer, MockProducer>();
            
            services.AddSingleton<IMessageBrokerConsumer, InteractiveConsumer>();
            services.AddSingleton<JobManipulationUseCase>();


            ConfigureCommonOptions(configuration, services);


            return services.BuildServiceProvider();
        }



        private static void ConfigureCommonOptions(IConfigurationRoot configuration, IServiceCollection services)
        {
            var rootName = "DataAcquisitionService";

            services.AddOptions<ComponentOptions>()
                .Bind(configuration.GetSection($"{rootName}:ComponentOptions"))
                .ValidateDataAnnotations();

            services.AddOptions<RegistrationRequestOptions>()
                .Bind(configuration.GetSection($"{rootName}:RegistrationRequestOptions"))
                .ValidateDataAnnotations();

            services.AddOptions<KafkaOptions>()
                .Bind(configuration.GetSection($"{rootName}:KafkaOptions"))
                .ValidateDataAnnotations();

            services.AddOptions<TwitterCredentialsOptions>()
                .Bind(configuration.GetSection($"Twitter:Credentials"))
                .ValidateDataAnnotations();
        }

        public static async Task MainAsync(string[] args)
        {
            var builtProvider = Build();

            var uc = builtProvider.GetRequiredService<JobManipulationUseCase>();

            await uc.SimpleStartStop();

            //var jobManager = builtProvider.GetRequiredService<IJobManager>();

            //var twitterCredentialsOptions = builtProvider.GetService<IOptions<TwitterCredentialsOptions>>();

            //var jobConfig = new DataAcquirerJobConfig()
            //{
            //    Attributes = new Dictionary<string, string>
            //    {
            //        {"TopicQuery", "capi hnizdo" },
            //        {"AccessToken", twitterCredentialsOptions.Value.AccessToken},
            //        {"AccessTokenSecret" , twitterCredentialsOptions.Value.AccessTokenSecret},
            //        {"ApiKey",  twitterCredentialsOptions.Value.ApiKey},
            //        {"ApiSecretKey", twitterCredentialsOptions.Value.ApiSecretKey},
            //    },
            //    JobId = Guid.NewGuid(),
            //    OutputMessageBrokerChannels = new string[] { "MOCK-Post-output" }
            //};
            //try
            //{
            //    await jobManager.StartNewJobAsync(jobConfig);
            //}
            //catch
            //{

            //}

            await Task.Delay(TimeSpan.FromHours(1));
        }
    }
}
