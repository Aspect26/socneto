using Domain;
using Domain.Abstract;
using Domain.Acquisition;
using Domain.JobConfiguration;
using Domain.JobManagement;
using Domain.Registration;
using Infrastructure.Kafka;
using Infrastructure.Twitter;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Domain.JobManagement.Abstract;
using Infrastructure.DataGenerator;
using System.Reflection;

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

            services.AddTransient<IDataAcquirer, TwitterDataAcquirer>();

            services.AddSingleton<TwitterContextProvider>();

            services.AddSingleton<JobConfigurationUpdateListenerHostedService>();

            services.AddTransient<IMessageBrokerProducer, FileProducer>();

            services.AddSingleton<IMessageBrokerConsumer, MockConsumer>();

            services.AddSingleton<IDataAcquirerJobStorage, DataAcquirerJobFileStorage>();
            services.AddSingleton(typeof(IEventTracker<>), typeof(NullEventTracker<>));
            // TW
            services.AddSingleton<IDataAcquirer, TwitterDataAcquirer>()
              .AddSingleton<IDataAcquirerMetadataContextProvider, TwitterMetadataContextProvider>()
              .AddSingleton<IDataAcquirerMetadataStorage, NullMetadataStorage>()
              .AddSingleton<IDataAcquirerMetadataContext, TwitterMetadataContext>()
                .AddSingleton<TwitterBatchLoaderFactory>();
            
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

            services.AddOptions<FileProducerOptions>()
                .Bind(configuration.GetSection($"{rootName}:FileProducerOptions"))
                .ValidateDataAnnotations();

            services.AddOptions<TwitterCredentialsOptions>()
                .Bind(configuration.GetSection($"Twitter:Credentials"))
                .ValidateDataAnnotations();

            // TW

            var assemblyPath = (new Uri(Assembly.GetExecutingAssembly().CodeBase)).AbsolutePath;
            var directory = new FileInfo(assemblyPath).Directory.FullName;
            var twitterMetaDir = Path.Combine(directory, "metatw");
            var jobMetaDir = Path.Combine(directory, "metajob");

            Directory.CreateDirectory(twitterMetaDir);
            Directory.CreateDirectory(jobMetaDir);

            services.AddOptions<TwitterJsonStorageOptions>()
                .Bind(configuration.GetSection($"{rootName}:TwitterJsonStorageOptions"))
                .PostConfigure(o => o.Directory = twitterMetaDir);

            services.AddOptions<DataAcquirerJobFileStorageOptions>()
                .Bind(configuration.GetSection($"{rootName}:DataAcquirerJobFileStorageOptions"))
                .PostConfigure(o => o.Directory = jobMetaDir);
        }

        public static async Task MainAsync(string[] args)
        {
            var builtProvider = Build();

            var jobManager = builtProvider.GetRequiredService<IJobManager>();

            var twitterCredentialsOptions = builtProvider.GetService<IOptions<TwitterCredentialsOptions>>();
            await StartJob(jobManager, twitterCredentialsOptions);



            await Task.Delay(TimeSpan.FromHours(1));
        }

        private static async Task StartJob(IJobManager jobManager, IOptions<TwitterCredentialsOptions> twitterCredentialsOptions)
        {

            // var query = "snakebite;snakebites;\"morsure de serpent\";\"morsures de serpents\";\"لدغات الأفاعي\";\"لدغة الأفعى\";\"لدغات أفاعي\";\"لدغة أفعى\"";
            // TODO add NOT cocktail NOT music
            // var query = "snake bite NOT cocktail NOT darts NOT piercing";
            var query = "iran";
            var jobConfig = new DataAcquirerJobConfig()
            {
                Attributes = new Dictionary<string, string>
                {
                    {"TopicQuery", query },
                    {"AccessToken", twitterCredentialsOptions.Value.AccessToken},
                    {"AccessTokenSecret" , twitterCredentialsOptions.Value.AccessTokenSecret},
                    {"ApiKey",  twitterCredentialsOptions.Value.ApiKey},
                    {"ApiSecretKey", twitterCredentialsOptions.Value.ApiSecretKey},
                },
                JobId = Guid.NewGuid(),
                OutputMessageBrokerChannels = new string[] { "job_management.component_data_input.DataAnalyser_sentiment" }
            };
            try
            {
                await jobManager.StartNewJobAsync(jobConfig);
            }
            catch
            {

            }
        }
    }
}
