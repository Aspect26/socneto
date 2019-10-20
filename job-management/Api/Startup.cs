using Domain.Abstract;
using Domain.ComponentManagement;
using Domain.Models;
using Domain.Registration;
using Domain.SubmittedJobConfiguration;
using Infrastructure;
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
        
        readonly string MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
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
            
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            services.AddHostedService<RegistrationRequestListenerHostedService>();
            services.AddTransient<RegistrationRequestListener>();
            services.AddTransient<IRegistrationRequestProcessor, RegistrationRequestProcessor>();
            services.AddTransient<ISubscribedComponentManager, SubscribedComponentManager>();
            services.AddTransient<IComponentConfigUpdateNotifier, ComponentConfigUpdateNotifier>();
#if DEBUG
            services.AddSingleton<IMessageBrokerConsumer, MockKafka>();
            services.AddTransient<IMessageBrokerProducer, MockKafka>();
#else
            services.AddTransient<IMessageBrokerConsumer, KafkaConsumer>();
            services.AddTransient<IMessageBrokerProducer, KafkaProducer>();
#endif
            services.AddSingleton<IComponentRegistry, ComponentStorageProxy>();
            services.AddSingleton<IMessageBrokerApi, KafkaApi>();

            services.Configure<RegistrationRequestOptions>(
                Configuration.GetSection("JobManagementService:RegistrationRequestOptions")
                );
            services.Configure<KafkaOptions>(
                Configuration.GetSection("JobManagementService:KafkaOptions")
            );

            services.AddOptions<ComponentIdentifiers>()
                .Bind(Configuration.GetSection("JobManagementService:ComponentIdentifiers"))
                .ValidateDataAnnotations();

            services.AddOptions<ComponentStorageOptions>()
                .Bind(Configuration.GetSection("JobManagementService:ComponentStorageOptions"))
                .ValidateDataAnnotations();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            
            app.UseCors(MyAllowSpecificOrigins);
            app.UseMvc();
        }
    }
}
