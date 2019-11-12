using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Domain;
using Domain.Abstract;
using Domain.JobConfiguration;
using Domain.JobManagement;
using Domain.JobManagement.Abstract;
using Domain.Registration;
using Infrastructure.DataGenerator;
using Infrastructure.Kafka;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog.Extensions.Logging;

namespace Application
{
    public class DataAcquisitionServiceWebApiBuilder
    {

        private readonly List<Action<IServiceCollection, IConfiguration>> _configurationActions
            = new List<Action<IServiceCollection, IConfiguration>>();
        private readonly List<Action<IServiceCollection>> _transientServices =
            new List<Action<IServiceCollection>>();
        private readonly List<Action<IServiceCollection>> _singletonServices =
            new List<Action<IServiceCollection>>();

        private readonly List<Action<IServiceCollection>> _postConfigureActions = 
            new List<Action<IServiceCollection>>();

        private readonly string[] _args;

        public DataAcquisitionServiceWebApiBuilder(string[] args)
        {
            _args = args;
        }

        public DataAcquisitionServiceWebApiBuilder ConfigureSpecificOptions<T>(string sectionName) where T : class
        {
            void ConfigurationAction(IServiceCollection serviceCollectino, IConfiguration c)
            {
                var sec = c.GetSection(sectionName);
                
                serviceCollectino.AddOptions<T>()
                    .Bind(sec)
                    .ValidateDataAnnotations();
            }

            _configurationActions.Add(ConfigurationAction);

            return this;
        }


        public DataAcquisitionServiceWebApiBuilder AddSingletonService<TAbstract, TConcrete>()
            where TAbstract : class
            where TConcrete : class, TAbstract
        {
            void addSingletonServiceAction(IServiceCollection sp)
            {
                sp.AddSingleton<TAbstract, TConcrete>();
            }

            _singletonServices.Add(addSingletonServiceAction);
            return this;
        }

        public DataAcquisitionServiceWebApiBuilder PostConfigure<TOptions>(Action<TOptions> action)
            where TOptions : class
        {
            void postConfigure(IServiceCollection sp)
            {
                sp.PostConfigure<TOptions>(action);
            }
            _postConfigureActions.Add(postConfigure);
            return this;
        }
        public DataAcquisitionServiceWebApiBuilder AddTransientService<TAbstract, TConcrete>()
            where TAbstract : class
            where TConcrete : class, TAbstract
        {
            void AddTransientServiceAction(IServiceCollection sp)
            {
                sp.AddTransient<TAbstract, TConcrete>();
            }

            _transientServices.Add(AddTransientServiceAction);
            return this;
        }

        


        public IWebHost BuildWebHost()
        {
            IWebHostEnvironment environment = null;

            var builder = new ConfigurationBuilder()
                           .SetBasePath(Directory.GetCurrentDirectory())
                           .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
            var aspNetCoreEnv = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            var isDevelopment = aspNetCoreEnv == "Development";

            if (isDevelopment)
            {
                builder.AddJsonFile($"appsettings.Development.json", true, true);
            }

            var configuration = builder.Build();
            var webHost = WebHost
                .CreateDefaultBuilder(_args)
                .ConfigureLogging(logging=>
                {
                    logging.AddFile("Logs/ts-{Date}.txt");
                    logging.AddConfiguration(configuration.GetSection("Logging"));
                })
                .ConfigureAppConfiguration((hostingContext, configurationBuilder) =>
                {
                    //rootConfiguration = configurationBuilder.Build();
                    environment = hostingContext.HostingEnvironment;
                })
                .ConfigureServices(services =>
                    ConfigureServices(services, configuration, isDevelopment))
                .Configure(app =>
                {
                    if (environment.IsDevelopment())
                    {
                        app.UseDeveloperExceptionPage();
                    }
                    
                    app.UseRouting();

                    app.UseEndpoints(endpoints =>
                    {
                        //endpoints.MapGet("/", async context =>
                        //{
                        //    await context.Response.WriteAsync("Hello World!");
                        //});
                    });
                })
                .Build();

            //test job replay

            ReplayJobConfigsAsync(webHost).GetAwaiter().GetResult();

            return webHost;
        }

        private static async Task ReplayJobConfigsAsync(IWebHost webHost)
        {
            var jm = webHost.Services.GetRequiredService<IJobManager>();
            var js = webHost.Services.GetRequiredService<IDataAcquirerJobStorage>();

            var jobs = await js.GetAllAsync();
            foreach (var job in jobs)
            {
                await jm.StartNewJobAsync(job);
            }
        }

        private void ConfigureServices(
            IServiceCollection services,
            IConfiguration configuration,
            bool isDevelopment)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_3_0);

            services.AddSingleton<JobConfigurationUpdateListener>();
            services.AddHostedService<JobConfigurationUpdateListenerHostedService>();

            services.AddSingleton<IJobManager, JobManager>();
            services.AddSingleton<IDataAcquirerJobStorage, DataAcquirerJobFileStorage>();

            services.AddTransient<IRegistrationService, RegistrationService>();

            if (isDevelopment)
            {
                services.AddTransient<IMessageBrokerProducer, MockProducer>();
                services.AddTransient<IMessageBrokerConsumer, MockConsumer>();
                services.AddOptions<MockConsumerOptions>()
                    .Bind(configuration.GetSection("DataAcquisitionService:MockConsumerOptions"))
                    .ValidateDataAnnotations();
            }
            else
            {
                services.AddTransient<IMessageBrokerProducer, KafkaProducer>();
                services.AddTransient<IMessageBrokerConsumer, KafkaConsumer>();
            }

            _transientServices.ForEach(addTransMethod => addTransMethod(services));
            _singletonServices.ForEach(addSingletonMehtod => addSingletonMehtod(services));

            ConfigureCommonOptions(configuration, services);

            _configurationActions.ForEach(specificConfigActions =>
                specificConfigActions(services, configuration));

            _postConfigureActions.ForEach(action =>
                action(services));            
        }

        private static void ConfigureCommonOptions(IConfiguration configuration, IServiceCollection services)
        {
            var rootName = "DataAcquisitionService";


            services.AddOptions<ComponentOptions>()
                .Bind(configuration.GetSection($"{rootName}:ComponentOptions"))
                .ValidateDataAnnotations()                ;

            services.AddOptions<RegistrationRequestOptions>()
                .Bind(configuration.GetSection($"{rootName}:RegistrationRequestOptions"))
                .ValidateDataAnnotations();

            services.AddOptions<KafkaOptions>()
                .Bind(configuration.GetSection($"{rootName}:KafkaOptions"))
                .ValidateDataAnnotations();


            services.AddOptions<MockConsumerOptions>()
                .Bind(configuration.GetSection("DataAcquisitionService:MockConsumerOptions"))
                .ValidateDataAnnotations();

        }


    }
}
