using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Domain;
using Domain.JobConfiguration;
using Domain.JobManagement;
using Domain.Registration;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Api
{
    public class Program
    {
        public static async Task MainAsync(string[] args)
        {
#if !DEBUG
            var delay = TimeSpan.FromMinutes(1);
            Console.WriteLine($"Waiting {delay}");
            await Task.Delay(delay);
#endif
            var app = CreateWebHostBuilder(args)
                .Build();

            await InitializeApplication(app);

            app.Run();
        }

        private static async Task InitializeApplication(IWebHost app)
        {
            var logger = app.Services.GetRequiredService<ILogger<Program>>();
            //logger.LogInformation("Starting to wait");
            //for (int i = 0; i < 3; i++)
            //{
            //    await Task.Delay(TimeSpan.FromSeconds(10));
            //    logger.LogInformation($"Waiting { TimeSpan.FromSeconds(10) * i }");
            //}


            await RegisterComponent(app, logger);

            StartListeningToJobConfigs(app);
        }

        private static void StartListeningToJobConfigs(IWebHost app)
        {
            var jobConfigurationUpdateListener = app.Services.GetRequiredService<JobConfigurationUpdateListener>();
            jobConfigurationUpdateListener.OnConnectionEstablished();
        }

        private static async Task RegisterComponent(IWebHost app, ILogger<Program> logger)
        {
            var registration = app.Services.GetRequiredService<IRegistrationService>();


            var componentOptions = app.Services.GetRequiredService<IOptions<ComponentOptions>>();


            var registrationRequest = new RegistrationRequest()
            {
                ComponentId = componentOptions.Value.ComponentId,
                ComponentType = componentOptions.Value.ComponentType,
                InputChannelName = componentOptions.Value.InputChannelName,
                UpdateChannelName = componentOptions.Value.UpdateChannelName
            };

            try
            {
                logger.LogInformation("Sending registration request");
                await registration.Register(registrationRequest);
                logger.LogInformation("Registration request sent");
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                throw;
            }

            logger.LogInformation("Service {serviceName} register request sent", "DataAcquisitionService");
        }

        public static void Main(string[] args)
        {
            MainAsync(args).GetAwaiter().GetResult();    
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>();


        
    }
}
