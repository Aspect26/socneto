using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;
using System.Threading.Tasks;
using LinqToTwitter;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SAnalyser.DataAcquisition.Domain;
using SAnalyser.DataAcquisition.Infrastructure;
using StructureMap;

namespace SAnalyser.DataAcquisition.ConsoleApp
{
    class Program
    {
        public static IConfigurationRoot Configuration;
        
        static void Main(string[] args)
        {
            #region config

            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.dev.json", optional: true, reloadOnChange: true);
            
            Configuration = builder.Build();
            #endregion

            // add the framework services
            var serviceProvider = new ServiceCollection()
                .AddLogging()
                .AddTransient<StuffDoer>()
                .AddTransient<ISocialNetwork,TwitterApi>()
                .AddTransient<TwitterContext>()
                .Configure<TwitterOptions>(Configuration.GetSection("Networks.Twitter"))
                .BuildServiceProvider();


            var services = new ServiceCollection()
                .AddLogging();

            // add StructureMap
            var container = new Container();
            container.Configure(config =>
            {
                // Register stuff in container, using the StructureMap APIs...
                config.Scan(_ =>
                {
                    _.AssemblyContainingType(typeof(Program));
                    _.WithDefaultConventions();
                });
                // Populate the container using the service collection
                config.Populate(services);
            });

            //var serviceProvider = container.GetInstance<IServiceProvider>();
            ////setup our DI
            //serviceProvider
            //    .AddTransient<StuffDoer>()
            //    .BuildServiceProvider();

            //configure console logging
            serviceProvider
                .GetService<ILoggerFactory>()
                
                .AddConsole(LogLevel.Debug);

            var logger = serviceProvider.GetService<ILoggerFactory>()
                .CreateLogger<Program>();
            logger.LogDebug("Starting application");

            //do the actual work here
            var bar = serviceProvider.GetService<StuffDoer>();

            var term = Console.ReadLine();
            var res = bar.DoSomeRealWork(term).GetAwaiter().GetResult();

            foreach (var post in res)
            {
                Console.WriteLine(post.Text);
            }

            Console.ReadLine();
            logger.LogDebug("All done!");
        }
    }
}
