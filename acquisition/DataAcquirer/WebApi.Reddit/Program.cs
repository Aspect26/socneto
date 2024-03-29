using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Application;
using Domain;
using Domain.Acquisition;
using Domain.JobConfiguration;
using Domain.JobManagement;
using Domain.Registration;
using Infrastructure.Reddit;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace WebApi.Reddit
{
    public class Program
    {
        public static async Task MainAsync(string[] args)
        {
            if (args.Contains("--sleep_on_startup"))
            {
                await Task.Delay(TimeSpan.FromSeconds(60));
            }

            //var assemblyPath = (new Uri(Assembly.GetExecutingAssembly().CodeBase)).AbsolutePath;

            var builder = new DataAcquisitionServiceWebApiBuilder(args)
                .AddSingletonService<IDataAcquirer, RedditDataAcquirer>()
                //.AddSingletonService<IDataAcquirerMetadataContextProvider, TwitterMetadataContextProvider>()
                //.AddSingletonService<IDataAcquirerMetadataStorage, TwitterJsonFileMetadataStorage>()
                .AddSingletonService<RedditContextProvider>()
            //    .ConfigureSpecificOptions<DataAcquirerJobFileStorageOptions>("DataAcquisitionService:DataAcquirerJobFileStorageOptions")
            //.PostConfigure<DataAcquirerJobFileStorageOptions>(o => o.Directory = jobMetaDir);
            ;

            var app = builder.BuildWebHost(false);
            await InitializeApplication(app);

            await app.RunAsync();
        }

        private static async Task InitializeApplication(IWebHost app)
        {
            var logger = app.Services.GetRequiredService<ILogger<Program>>();

            await RegisterComponentAsync(app, logger);
        }


        private static async Task RegisterComponentAsync(IWebHost app, ILogger<Program> logger)
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
                    await Task.Delay(TimeSpan.FromMinutes(.5));
                    await registration.Register(registrationRequest);
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
