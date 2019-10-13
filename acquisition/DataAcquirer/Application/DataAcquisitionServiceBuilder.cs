using Domain;
using Domain.Abstract;
using Domain.Acquisition;
using Domain.JobConfiguration;
using Domain.JobManagement;
using Domain.Registration;
using Infrastructure.Kafka;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;

namespace Application
{
    public class DataAcquisitionConsoleAppBuilder
    {

        private List<Action<IServiceCollection, IConfigurationRoot>> _configurationActions 
            = new List<Action<IServiceCollection, IConfigurationRoot>>();
        private readonly List<Action<IServiceCollection>> _transientServices =
            new List<Action<IServiceCollection>>();
        private readonly List<Action<IServiceCollection>> _singletonServices =
            new List<Action<IServiceCollection>>();

        
        public IServiceProvider Build()
        {
            var builder = new ConfigurationBuilder()
                           .SetBasePath(Directory.GetCurrentDirectory())
                           .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
            var aspNetCoreEnv = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            if( aspNetCoreEnv == "Development")
            {
                builder.AddJsonFile($"appsettings.Development.json", true, true);
            }
            var configuration = builder.Build();


            var services = new ServiceCollection();
            services.AddLogging(
                logging => logging
                .AddConsole()
                .SetMinimumLevel(LogLevel.Information));

            // add the framework services
            services.AddSingleton<JobConfigurationUpdateListener>();
            services.AddHostedService<JobConfigurationUpdateListenerHostedService>();

            services.AddTransient<IJobManager, JobManager>();

            services.AddTransient<IRegistrationService, RegistrationService>();

            services.AddTransient<IMessageBrokerProducer, MockProducer>();
            services.AddTransient<IMessageBrokerConsumer, MockConsumer>();

            _transientServices.ForEach(addTransMethod => addTransMethod(services));
            _singletonServices.ForEach(addSingletonMehtod => addSingletonMehtod(services));

            ConfigureCommonOptions(configuration, services);
            _configurationActions.ForEach(specificConfigActions =>
                specificConfigActions(services, configuration));


            return services.BuildServiceProvider();
        }

        public DataAcquisitionConsoleAppBuilder ConfigureSpecificOptions<T>(string sectionName) where T : class
        {
            void configurationAction(IServiceCollection sc, IConfigurationRoot c)
            {
                var sec = c.GetSection(sectionName);

                OptionsConfigurationServiceCollectionExtensions.Configure<T>(sc, sec);
            }

            _configurationActions.Add(configurationAction);

            return this;
        }


        public DataAcquisitionConsoleAppBuilder AddSingletonService<TAbstract, TConcrete>()
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
        public DataAcquisitionConsoleAppBuilder AddTransientService<TAbstract,TConcrete>() 
            where TAbstract:class
            where TConcrete:class,TAbstract
        {
            void addTransientServiceAction (IServiceCollection sp )
            {
                sp.AddTransient<TAbstract, TConcrete>();
            }

            _transientServices.Add(addTransientServiceAction);
            return this;
        }

        private static void ConfigureCommonOptions(IConfigurationRoot configuration, IServiceCollection services)
        {
            var rootName = "DataAcquisitionService";

            services.Configure<ComponentOptions>(
                configuration.GetSection($"{rootName}:ComponentOptions"));

            services.Configure<RegistrationRequestOptions>(
                configuration.GetSection($"{rootName}:RegistrationRequestOptions"));

            services.Configure<KafkaOptions>(
                configuration.GetSection($"{rootName}:KafkaOptions"));



            
        }
    }
}
