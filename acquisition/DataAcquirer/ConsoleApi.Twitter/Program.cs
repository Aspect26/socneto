using Application;
using Domain;
using Domain.Abstract;
using Domain.Acquisition;
using Domain.JobConfiguration;
using Domain.JobManagement;
using Domain.Model;
using Domain.Registration;
using Infrastructure.Kafka;
using Infrastructure.Twitter;
using LinqToTwitter;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Domain.JobManagement.Abstract;

namespace ConsoleApi.Twitter
{
    public class Program
    {
        public static void Main(string[] args)
        {
            MainAsync(args).GetAwaiter().GetResult();
        }
        public static async Task MainAsync(string[] args)
        {
            var builtProvider = new DataAcquisitionConsoleAppBuilder()
                .AddTransientService<IDataAcquirer, TwitterDataAcqirer>()
                .ConfigureSpecificOptions<TwitterCredentialsOptions>($"Twitter:Credentials")
                .Build();

            var jobManager = builtProvider.GetRequiredService<IJobManager>();

            var twitterCredentialsOptions = builtProvider.GetService<IOptions<TwitterCredentialsOptions>>();

            var jobConfig = new DataAcquirerJobConfig()
            {
                Attributes = new Dictionary<string, string>
                {
                    {"TopicQuery", "capi hnizdo" },
                    {"AccessToken", twitterCredentialsOptions.Value.AccessToken},
                    {"AccessTokenSecret" , twitterCredentialsOptions.Value.AccessTokenSecret},
                    {"ApiKey",  twitterCredentialsOptions.Value.ApiKey},
                    {"ApiSecretKey", twitterCredentialsOptions.Value.ApiSecretKey},
                },
                JobId = Guid.NewGuid(),
                OutputMessageBrokerChannels = new string[] { "MOCK-Post-output" }
            };
            try
            {
                await jobManager.StartNewJobAsync(jobConfig);
            }
            catch
            {

            }

            await Task.Delay(TimeSpan.FromHours(1));
        }        
    }
}
