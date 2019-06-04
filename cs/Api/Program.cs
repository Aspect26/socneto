using System.Net;
using System.Reflection;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;

namespace Socneto.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args)
        {
            int port = 5000;
            if (args.Length > 0)
            {
                port = int.Parse(args[0].Split(":")[2]);
            }

            return WebHost.CreateDefaultBuilder(args)
                .UseKestrel(options => {
                    options.Listen(IPAddress.Loopback, port); //HTTP port
                })
                .UseStartup<Startup>();

        }
            
            
    }
}
