using Api.Models;
using Domain.Abstract;
using Infrastructure.Kafka;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleApi.JobInvoker
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
                Path.Combine(Directory.GetCurrentDirectory(),"output_data"),
                "job_management.component_data_input.storage_db",
                "job_management.component_data_analyzed_input.storage_db"
            };
#endif

            var services = Configure();
            var jobInvoker = services.GetRequiredService<Invoker>();

            var commandReader = services.GetRequiredService<CommandFileReader>();

            var commands = await commandReader.ReadCommandsAsync<JobSubmitRequest>(args[0]);
            var commandTasks = commands.ToList().Select(jobInvoker.InvokeCommand);
            await Task.WhenAll(commandTasks);

            var baseDirectory = args[1];

            
            var dataJobInfo = GetDataInfo(baseDirectory, 
                "data", 
                args[2]);

            var analysisJobInfo = GetDataInfo(baseDirectory,
                "analysis",
                args[3]);

            await SaveData(services, dataJobInfo, analysisJobInfo);
        }

        private static (string topicName, DirectoryInfo dataJobDir) GetDataInfo(
            string baseDirectory, 
            string dirName, 
            string topicName)
        {
            var jobDirPath = Path.Combine(baseDirectory, dirName);
            var jobDir = new DirectoryInfo(jobDirPath);
            jobDir.Create();
            return (topicName, jobDir);
        }

        public static async Task SaveData(IServiceProvider services,
            ValueTuple<string, DirectoryInfo> dataJobInfo,
            ValueTuple<string, DirectoryInfo> analysisJobInfo)
        {
            var dataSaver = services.GetRequiredService<DataSaver>();

            var saveDataTask = Task.Run(async () => await dataSaver.ListenAndSaveAsync(
                dataJobInfo.Item1,
                dataJobInfo.Item2,
                CancellationToken.None));

            var saveAnalysisTask = Task.Run(async () => await dataSaver.ListenAndSaveAsync(
                analysisJobInfo.Item1,
                analysisJobInfo.Item2,
                CancellationToken.None));

            await Task.WhenAll(new[] { saveAnalysisTask, saveAnalysisTask });
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


            services.AddTransient<IMessageBrokerProducer, KafkaProducer>();

            services.AddTransient<IMessageBrokerConsumer, KafkaConsumer>();

            services.AddSingleton<DataSaver>();
            services.AddSingleton<CommandFileReader>();
            services.AddHttpClient<Invoker>();

            services.AddOptions<KafkaOptions>()
                .Bind(configuration.GetSection($"KafkaOptions"))
                .ValidateDataAnnotations();
            
                services.AddOptions<JobInvokerOptions>()
                .Bind(configuration.GetSection($"JobInvokerOptions"))
                .ValidateDataAnnotations();
            return services.BuildServiceProvider();
        }

    }

    public class JobInvokerOptions
    {
        [Required]
        public Uri JobSubmitUri { get; set; }
    }
    public class Invoker
    {
        private readonly JobInvokerOptions _jobInvokerOptions;
        private readonly HttpClient _httpClient;
        private readonly ILogger<Invoker> _logger;

        public Invoker(
            IOptions<JobInvokerOptions> jobInvokerOptions,
            HttpClient httpClient,
            ILogger<Invoker> logger)
        {
            _jobInvokerOptions = jobInvokerOptions.Value;
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task InvokeCommand(JobSubmitRequest request)
        {
            var contentJson = JsonConvert.SerializeObject(request);
            var stringContent = new StringContent(contentJson, Encoding.UTF8, "application/json");
            using (var r = await _httpClient.PostAsync(_jobInvokerOptions.JobSubmitUri, stringContent))
            {
                
                if(!r.IsSuccessStatusCode)
                {
                    var err = await r.Content.ReadAsStringAsync();
                    _logger.LogError("Error on jms side: {error}", err);
                }
            }
        }
    }

}
