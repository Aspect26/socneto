﻿using Domain.Abstract;
using Domain.ComponentManagement;
using Domain.JobStorage;
using Domain.Models;
using Domain.Registration;
using Domain.SubmittedJobConfiguration;
using Infrastructure;
using Infrastructure.ComponentManagement;
using Infrastructure.Kafka;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        private const string MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddRazorPages();
            services.AddCors(options =>
            {
                options.AddPolicy(MyAllowSpecificOrigins,
                    builder =>
                    {
                        builder.WithOrigins("*")
                            .AllowAnyHeader()
                            .AllowAnyMethod();
                    });
            });

            
            services.AddHostedService<RegistrationRequestListenerHostedService>();
            services.AddTransient<RegistrationRequestListener>();

#warning using in memory storage
            // services.AddHttpClient<IJobStorage, JobStorageProxy>();
            services.AddSingleton<IJobStorage, InMemoryJobStorage>();
            services.AddTransient<IRegistrationRequestProcessor, RegistrationRequestProcessor>();
            services.AddTransient<ISubscribedComponentManager, SubscribedComponentManager>();
            services.AddTransient<IComponentConfigUpdateNotifier, ComponentConfigUpdateNotifier>();
#if DEBUGx
            services.AddSingleton<IMessageBrokerConsumer, MockKafka>();
            services.AddTransient<IMessageBrokerProducer, MockKafka>();
            services.AddHttpClient<IComponentRegistry, ComponentStorageProxyMock>();
#else
            services.AddTransient<IMessageBrokerConsumer, KafkaConsumer>();
            services.AddTransient<IMessageBrokerProducer, KafkaProducer>();
            //services.AddHttpClient<IComponentRegistry, ComponentStorageProxy>();
#warning using in memory storage
            services.AddSingleton<IComponentRegistry, InMemoryRegistry>();
#endif
            services.AddSingleton<IMessageBrokerApi, KafkaApi>();

            var optionRootName = "JobManagementService";
            services.AddOptions<RegistrationRequestOptions>()
                .Bind(Configuration.GetSection($"{optionRootName}:RegistrationRequestOptions"))
                .ValidateDataAnnotations();
            services.AddOptions<KafkaOptions>()
                .Bind(Configuration.GetSection($"{optionRootName}:KafkaOptions"))
                .ValidateDataAnnotations();

            services.AddOptions<ComponentIdentifiers>()
                .Bind(Configuration.GetSection($"{optionRootName}:ComponentIdentifiers"))
                .ValidateDataAnnotations();

            services.AddOptions<StorageChannelNames>()
                .Bind(Configuration.GetSection($"{optionRootName}:StorageChannelNames"))
                .ValidateDataAnnotations();

            services.AddOptions<ComponentStorageOptions>()
                .Bind(Configuration.GetSection($"{optionRootName}:ComponentStorageOptions"))
                .ValidateDataAnnotations();

            services.AddOptions<RegistrationRequestValidationOptions>()
                .Bind(Configuration.GetSection($"{optionRootName}:RegistrationRequestValidationOptions"))
                .ValidateDataAnnotations();

            services.AddOptions<JobStorageOptions>()
                .Bind(Configuration.GetSection($"{optionRootName}:JobStorageOptions"))
                .ValidateDataAnnotations();
            
            services.AddOptions<JobStorageOptions>()
                .Bind(Configuration.GetSection($"{optionRootName}:JobStorageOptions"))
                .ValidateDataAnnotations();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseCors(MyAllowSpecificOrigins);

            app.UseRouting();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
