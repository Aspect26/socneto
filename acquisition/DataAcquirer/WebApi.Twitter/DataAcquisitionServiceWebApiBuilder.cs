using System;
using System.Collections.Generic;
using System.IO;
using Domain;
using Domain.Abstract;
using Domain.JobConfiguration;
using Domain.JobManagement;
using Domain.Registration;
using Infrastructure.DataGenerator;
using Infrastructure.Kafka;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Api
{
    public class DataAcquisitionServiceWebApiBuilder
    {

        private List<Action<IServiceCollection, IConfiguration>> _configurationActions
            = new List<Action<IServiceCollection, IConfiguration>>();
        private readonly List<Action<IServiceCollection>> _transientServices =
            new List<Action<IServiceCollection>>();
        private readonly List<Action<IServiceCollection>> _singletonServices =
            new List<Action<IServiceCollection>>();
        private readonly string[] _args;

        public DataAcquisitionServiceWebApiBuilder(string[] args)
        {
            _args = args;
        }

        public DataAcquisitionServiceWebApiBuilder ConfigureSpecificOptions<T>(string sectionName) where T : class
        {
            void configurationAction(IServiceCollection sc, IConfiguration c)
            {
                var sec = c.GetSection(sectionName);

                OptionsConfigurationServiceCollectionExtensions.Configure<T>(sc, sec);
            }

            _configurationActions.Add(configurationAction);

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
        public DataAcquisitionServiceWebApiBuilder AddTransientService<TAbstract, TConcrete>()
            where TAbstract : class
            where TConcrete : class, TAbstract
        {
            void addTransientServiceAction(IServiceCollection sp)
            {
                sp.AddTransient<TAbstract, TConcrete>();
            }

            _transientServices.Add(addTransientServiceAction);
            return this;
        }



        public IWebHost BuildWebHost()
        {
            IHostingEnvironment environment = null;

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
                    app.UseMvc();
                    //app.UseAuthentication();
                    //app.UseMiddleware<ExceptionLoggingMiddleware>();

                })
                .Build();
            return webHost;
        }

        private void ConfigureServices(
            IServiceCollection services,
            IConfiguration configuration,
            bool isDevelopment)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            services.AddSingleton<JobConfigurationUpdateListener>();
            services.AddHostedService<JobConfigurationUpdateListenerHostedService>();

            services.AddTransient<IJobManager, JobManager>();

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
        }

        private static void ConfigureCommonOptions(IConfiguration configuration, IServiceCollection services)
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

        }


    }
}
