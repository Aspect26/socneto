using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Domain.Models;
using Infrastructure;
using Infrastructure.Kafka;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace TestCases
{
    public class ProducerTestCase:ITestCase
    {
        public async Task PerformTestCase()
        {
            var services = Configure();
            var producer = services.GetService<KafkaProducer>();


            var message = new Message()
            {
                Key = "somekey",
                Value = "somevalue"
            };
            await producer.ProduceAsync("testTopic", message);
        }

        static IServiceProvider Configure()
        {
            var builder = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                ;

            var configuration = builder.Build();

            
            // add the framework services
            var services = new ServiceCollection()
                .AddLogging(r => r.AddConsole());
            
            services.AddTransient<KafkaProducer>();
            services.AddTransient<IOptions<KafkaOptions>>(
                sp => Options.Create<KafkaOptions>(
                    new KafkaOptions()
                    {
                        ServerAddress = "acheron.ms.mff.cuni.cz:39108"
                    }
                )
            );

            //services.Configure<TestCaseOptions>(configuration.GetSection("Datamole.DataPlatform.Gateway:TestCaseOptions"));

            var builtProvider = services.BuildServiceProvider();

            return builtProvider;
        }
    }
}
