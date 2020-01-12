using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Api
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            if (args.Contains("--sleep_on_startup"))
            {
                await Task.Delay(TimeSpan.FromSeconds(180));
            }
            
            var app = CreateWebHostBuilder(args).Build();
            app.Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args)
        {
            return WebHost.CreateDefaultBuilder(args)
               .UseStartup<Startup>();
        }
    }
}
