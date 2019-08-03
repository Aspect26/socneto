using Domain;
using Domain.Abstract;
using Domain.Analyser;
using Domain.JobConfiguration;
using Domain.PostAnalysis;
using Domain.Registration;
using Infrastructure.Kafka;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
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

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            services.AddSingleton<NewPostToAnalyzeListener>();
            services.AddHostedService<NewPostToAnalyzerLestenerHostedService>();

            services.AddSingleton<JobConfigurationUpdateListener>();
            services.AddHostedService<JobConfigurationUpdateListenerHostedService>();
            
            services.AddSingleton<IJobManager, JobManager>();
            services.AddTransient<IAnalyser, MockAnalyser>();
            services.AddTransient<IRegistrationService, RegistrationService>();

#if DEBUG
            services.AddTransient<IMessageBrokerProducer, MockProducer>();
            services.AddTransient<IMessageBrokerConsumer, MockConsumer>();
#else
            services.AddTransient<IMessageBrokerProducer, KafkaProducer>();
           services.AddTransient<IMessageBrokerConsumer, KafkaConsumer>();
#endif
            services.Configure<ComponentOptions>(
                Configuration.GetSection("DataAcquisitionService:ComponentOptions"));

            services.Configure<RegistrationRequestOptions>(
                Configuration.GetSection("DataAcquisitionService:RegistrationRequestOptions"));

            services.Configure<KafkaOptions>(
                Configuration.GetSection("DataAcquisitionService:KafkaOptions"));
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
