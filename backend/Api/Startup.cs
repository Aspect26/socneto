using Bazinga.AspNetCore.Authentication.Basic;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Socneto.Api.Authentication;
using Socneto.Domain;
using Socneto.Domain.Services;
using Socneto.Infrastructure;
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
                        builder.WithOrigins("*")
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

            services
#if DEBUG
                .AddTransient<IResultProducer, MockKafka>()
#else
                .AddTransient<IResultProducer, KafkaProducer>()
#endif
                .AddTransient<IAuthorizationService, AuthorizationService>()
                .AddTransient<IJobService, JobService>()
                .AddTransient<IUserService, UserService>()
                .AddTransient<IGetAnalysisService, GetAnalysisService>()
                .AddTransient<IChartsService, ChartsService>()
                .AddTransient<IJobManagementService, JobManagementService>()
                .AddTransient<IStorageService, StorageService>()
                .Configure<TaskOptions>(Configuration.GetSection("Socneto:TaskOptions"))
                .Configure<KafkaOptions>(Configuration.GetSection("Socneto:KafkaOptions"))
                .Configure<JMSOptions>(Configuration.GetSection("Socneto:JobManagementServiceOptions"))
                .Configure<StorageOptions>(Configuration.GetSection("Socneto:StorageOptions"));
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

            app.UseCors(MyAllowSpecificOrigins);

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
