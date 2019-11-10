using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Application;
using Domain;
using Domain.Acquisition;
using Domain.JobConfiguration;
using Domain.JobManagement;
using Domain.Registration;
using Infrastructure.DataGenerator;
using Infrastructure.StaticData;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Api
{
    public class Program
    {
        public static async Task MainAsync(string[] args)
        {
            var app = new DataAcquisitionServiceWebApiBuilder(args)
                .ConfigureSpecificOptions<RandomGeneratorOptions>("DataAcquisitionService:RandomGeneratorOptions")
                .ConfigureSpecificOptions<StaticDataOptions>("DataAcquisitionService:StaticGeneratorOptions")
                .AddSingletonService<IStaticDataProvider, MovieDataProvider>()
                .AddSingletonService<IDataAcquirer,StaticDataEnumerator>()
                .BuildWebHost();

            await InitializeApplication(app);

            app.Run();
        }

        private static async Task InitializeApplication(IWebHost app)
        {
            var logger = app.Services.GetRequiredService<ILogger<Program>>();

            await RegisterComponent(app, logger);
            
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

            while (true)
            {
                try
                {
                    logger.LogInformation("Sending registration request");
                    await registration.Register(registrationRequest);
                    logger.LogInformation("Service {serviceName} register request sent", "DataAcquisitionService");
                    break;
                }
                catch (Exception e)
                {
                    logger.LogError("Registration failed. Error: {error}", e.Message);
                    logger.LogInformation("trying again in 30 seconds");
                    await Task.Delay(TimeSpan.FromMinutes(.5));
                }
            }
        }

        public static void Main(string[] args)
        {
            MainAsync(args).GetAwaiter().GetResult();    
        }        
    }
}
