using System;

namespace ConsoleApi
{
    class Program
    {
        static void Main(string[] args)
        {
            
            //var builder = new ConfigurationBuilder()
            //    .SetBasePath(Directory.GetCurrentDirectory())
            //    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

            //Configuration = builder.Build();
            

            //// add the framework services
            //var serviceProvider = new ServiceCollection()
            //    .AddLogging(logging => logging.AddConsole().SetMinimumLevel(LogLevel.Information))
            //    .AddTransient<KafkaConsumer>()
            //    .AddTransient<KafkaProducer>()
            //    .AddTransient<SmokeTester>()
            //    .AddTransient<CoordinatorClient>()
            //    .Configure<TaskOptions>(Configuration.GetSection("SmokeTester:TaskOptions"))
            //    .Configure<KafkaOptions>(Configuration.GetSection("SmokeTester:KafkaOptions"))
            //    .Configure<SmokeTesterOptions>(Configuration.GetSection("SmokeTester:SmokeTesterOptions"))
            //    .BuildServiceProvider();




            //var auth = new SingleUserAuthorizer
            //{
            //    CredentialStore = new SingleUserInMemoryCredentialStore
            //    {
            //        ConsumerKey = ConfigurationManager.AppSettings["consumerKey"],
            //        ConsumerSecret = ConfigurationManager.AppSettings["consumerSecret"],
            //        AccessToken = ConfigurationManager.AppSettings["accessToken"],
            //        AccessTokenSecret = ConfigurationManager.AppSettings["accessTokenSecret"]
            //    }
            //};


        }
    }
}
