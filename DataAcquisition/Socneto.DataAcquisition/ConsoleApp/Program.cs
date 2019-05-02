using System.IO;
using LinqToTwitter;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Socneto.DataAcquisition.Domain;
using Socneto.DataAcquisition.Infrastructure;
using Socneto.DataAcquisition.Infrastructure.Kafka;

namespace Socneto.DataAcquisition.ConsoleApp
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
                .AddLogging(logging=>logging.AddConsole().SetMinimumLevel(LogLevel.Information) )
                .AddTransient<StuffDoer>()
                .AddTransient<IJobConsumer, KafkaConsumer>()
                .AddTransient<IResultProducer, KafkaProducer>()
                .Configure<TaskOptions>(Configuration.GetSection("Coordinator:TaskOptions"))
                .Configure<KafkaOptions>(Configuration.GetSection("Coordinator:KafkaOptions"))
                .BuildServiceProvider();

            
            //do the actual work here

            serviceProvider.GetService<StuffDoer>()
                .DoSomeRealWork().GetAwaiter().GetResult();
            
            
            
            
        }
    }
}
