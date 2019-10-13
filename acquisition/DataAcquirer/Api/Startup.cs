using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain;
using Domain.Abstract;
using Domain.Acquisition;
using Domain.JobConfiguration;
using Domain.JobManagement;
using Domain.Registration;
using Infrastructure.Kafka;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            services.AddSingleton<JobConfigurationUpdateListener>();
            services.AddHostedService<JobConfigurationUpdateListenerHostedService>();
            
            services.AddTransient<IJobManager,JobManager>();

            services.AddTransient<IRegistrationService, RegistrationService>();
#if DEBUG
            services.AddTransient<IMessageBrokerProducer, MockProducer>();
            services.AddTransient<IMessageBrokerConsumer, MockConsumer>();
#else
            services.AddTransient<IMessageBrokerProducer, KafkaProducer>();
           services.AddTransient<IMessageBrokerConsumer, KafkaConsumer>();
#endif

            var rootName = "TwitterDataAcquisitionService";

            services.Configure<ComponentOptions>(
                Configuration.GetSection($"{rootName}:ComponentOptions"));

            services.Configure<RegistrationRequestOptions>(
                Configuration.GetSection($"{rootName}:RegistrationRequestOptions"));

            services.Configure<KafkaOptions>(
                Configuration.GetSection($"{rootName}:KafkaOptions"));


        }
        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            
            app.UseMvc();
        }
    }
}
