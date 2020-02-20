using System;
using Bazinga.AspNetCore.Authentication.Basic;
using Domain.EventTracking;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Socneto.Api.Authentication;
using Socneto.Domain;
using Socneto.Domain.EventTracking;
using Socneto.Domain.Services;
using Socneto.Infrastructure.Kafka;
using Swashbuckle.AspNetCore.Swagger;

namespace Socneto.Api
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
                        builder.WithOrigins("http://acheron.ms.mff.cuni.cz:39110", "http://acheron.ms.mff.cuni.cz:39103", "http://acheron.ms.mff.cuni.cz:39109", "http://localhost:8080")
                               .AllowAnyHeader()
                               .AllowAnyMethod();
                    });
            });
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info
                {
                    Version = "v1",
                    Title = "Socneto Api",
                    Description = "Socneto task api - no users yet"
                });
            });

            services.AddAuthentication(BasicAuthenticationDefaults.AuthenticationScheme)
                .AddBasicAuthentication<SimpleBasicCredentialVerifier>();

            var noKafka = false;
            // TODO use arguments instead of symbol
#if DEBUG
            noKafka = true;
#endif
            if (noKafka)
            {
                services
                    .AddTransient<IResultProducer, MockResultProducer>()
                    .AddTransient<IMessageBrokerProducer, MockKafka>()
                    .AddSingleton(typeof(IEventTracker<>), typeof(ConsoleEventTracker<>));
            }
            else
            {
                services
                    .AddTransient<IResultProducer, KafkaResultProducer>()
                    .AddTransient<IMessageBrokerProducer, KafkaProducer>()
                    .AddSingleton(typeof(IEventTracker<>), typeof(EventTracker<>));
            }
            services.AddTransient<IAuthorizationService, AuthorizationService>()
                .AddTransient<IJobService, JobService>()
                .AddTransient<IUserService, UserService>()
                .AddTransient<IGetAnalysisService, GetAnalysisService>()
                .AddTransient<IChartsService, ChartsService>()
                .AddTransient<IJobManagementService, JobManagementService>()
                .AddTransient<IStorageService, StorageService>()
                .AddTransient<ICsvService, CsvService>()
                .AddHostedService<EventSendingHostedService>()
                .AddSingleton<EventQueue>()
                .Configure<TaskOptions>(Configuration.GetSection("Socneto:TaskOptions"))
                .Configure<KafkaOptions>(Configuration.GetSection("Socneto:KafkaOptions"))
                .Configure<JMSOptions>(Configuration.GetSection("Socneto:JobManagementServiceOptions"))
                .Configure<StorageOptions>(Configuration.GetSection("Socneto:StorageOptions"))
                .Configure<ComponentOptions>(Configuration.GetSection("Socneto:ComponentOptions"))
                .Configure<SystemMetricsOptions>(Configuration.GetSection("Socneto:SystemMetricsOptions"))
                .Configure<DefaultAcquirersCredentialsOptions>(Configuration.GetSection("Socneto:DefaultAcquirersCredentials"));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseCors("_myAllowSpecificOrigins");

            //app.UseForwardedHeaders(new ForwardedHeadersOptions
            //{
            //    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
            //});

            app.UseAuthentication();

            //app.UseHttpsRedirection();

            app.UseMvc();

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Socneto Api");
            });

        }
    }
}
