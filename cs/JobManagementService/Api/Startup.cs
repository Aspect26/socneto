using Domain.Abstract;
using Domain.ComponentManagement;
using Domain.Registration;
using Domain.SubmittedJobConfiguration;
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
            
            services.AddHostedService<RegistrationRequestListenerHostedService>();
            services.AddTransient<RegistrationRequestListener>();
            services.AddTransient<IRegistrationRequestProcessor, RegistrationRequestProcessor>();
            services.AddTransient<IMessageBrokerConsumer, KafkaConsumer>();
            services.AddTransient<ISubscribedComponentManager, SubscribedComponentManager>();
            services.AddTransient<IComponentConfigUpdateNotifier, ComponentConfigUpdateNotifier>();
            services.AddTransient<IMessageBrokerProducer, KafkaProducer>();
            services.AddSingleton<IComponentRegistry, ComponentRegistry>();
            services.AddSingleton<IMessageBrokerApi, KafkaApi>();
            
            services.Configure<RegistrationRequestOptions>(
                Configuration.GetSection("JobManagementService:RegistrationRequestOptions")
                );
            services.Configure<KafkaOptions>(
                Configuration.GetSection("JobManagementService:KafkaOptions")
            );
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
