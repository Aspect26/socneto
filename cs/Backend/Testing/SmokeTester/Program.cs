using System.IO;
using LinqToTwitter;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Socneto.DataAcquisition.Domain;
using Socneto.DataAcquisition.Infrastructure;
using Socneto.DataAcquisition.Infrastructure.Kafka;


namespace SmokeTester
{
    class Program
    {
        public static IConfigurationRoot Configuration;
        static void Main(string[] args)
        {

            #region config

            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

            Configuration = builder.Build();
            #endregion

            // add the framework services
            var serviceProvider = new ServiceCollection()
                .AddLogging(logging => logging.AddConsole().SetMinimumLevel(LogLevel.Information))
                .AddTransient<KafkaConsumer>()
                .AddTransient< KafkaProducer>()
                .AddTransient<SmokeTester>()
                .AddTransient< CoordinatorClient>()
                .Configure<TaskOptions>(Configuration.GetSection("SmokeTester:TaskOptions"))
                .Configure<KafkaOptions>(Configuration.GetSection("SmokeTester:KafkaOptions"))
                .Configure<SmokeTesterOptions>(Configuration.GetSection("SmokeTester:SmokeTesterOptions"))
                .BuildServiceProvider();


            //do the actual work here
            
            serviceProvider.GetService<SmokeTester>()
                .Test().GetAwaiter().GetResult();
        }
    }
}
