using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Domain;
using Domain.Registration;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Api
{
    public class Program
    {
        public static async Task MainAsync(string[] args)
        {
            var app = CreateWebHostBuilder(args)
                .Build();

            await InitializeApplication(app);

            app.Run();
        }

        private static async Task InitializeApplication(IWebHost app)
        {
            var registration = app.Services.GetRequiredService<IRegistrationService>();

            var componentOptions = app.Services.GetRequiredService<IOptions<ComponentOptions>>();
            
            var registrationRequest = new RegistrationRequest()
            {
                ComponentId = componentOptions.Value.ComponentId,
                ComponentType = componentOptions.Value.ComponentType
            };

            await registration.Register(registrationRequest);
        }

        public static void Main(string[] args)
        {
            MainAsync(args).GetAwaiter().GetResult();    
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>();
    }
}
