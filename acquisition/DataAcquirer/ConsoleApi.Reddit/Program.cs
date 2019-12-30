using Application;
using Domain;
using Domain.Abstract;
using Domain.Acquisition;
using Domain.JobConfiguration;
using Domain.JobManagement;
using Domain.JobManagement.Abstract;
using Domain.Model;
using Domain.Registration;
using Infrastructure.DataGenerator;
using Infrastructure.Kafka;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Reddit;
using Reddit.Controllers;
using Reddit.Inputs;
using Reddit.Inputs.Search;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleApi.Reddit
{
    public class RedditCredentials
    {
        public RedditCredentials(
            string appId,
            string apiSecret,
            string refreshToken)
        {
            AppId = appId;
            ApiSecret = apiSecret;
            RefreshToken = refreshToken;
        }

        public string AppId { get; }
        public string ApiSecret { get; }
        public string RefreshToken { get; }
    }

    public class RedditContextProvider
    {
        private readonly ConcurrentDictionary<string, RedditClient> _redditClients =
            new ConcurrentDictionary<string, RedditClient>();
        private readonly IEventTracker<RedditContextProvider> _eventTracker;

        public RedditContextProvider(
            IEventTracker<RedditContextProvider> eventTracker)
        {
            _eventTracker = eventTracker;
        }
        public Task<RedditClient> GetContextAsync(RedditCredentials credentials)
        {
            credentials = credentials ?? throw new ArgumentNullException(nameof(credentials));

            var props = new[]
            {
                credentials.AppId,
                credentials.ApiSecret,
                credentials.RefreshToken
            };
            if (props.Any(r => string.IsNullOrEmpty(r)))
            {
                throw new ArgumentException("Credentials field contains null value", nameof(credentials));
            }

            var key = string.Join('+', props);
            if (_redditClients.TryGetValue(key, out var context))
            {
                return Task.FromResult(context);
            }

            try
            {
                var redditContext = new RedditClient(
                    appId: credentials.AppId,
                    appSecret: credentials.ApiSecret,
                    refreshToken: credentials.RefreshToken);

                var searchInput = new SearchGetSearchInput(q: "foo bar", count: 5);
                var s = redditContext.Search(searchInput);

                _redditClients.TryAdd(key, redditContext);
                return Task.FromResult(redditContext);

            }
            catch (Exception e)
            {
                _eventTracker.TrackError(
                    "RedditContext",
                    "Encountered error while creating context",
                    new
                    {
                        exception = e
                    });
                throw;
            }
        }

    }

    public class RedditDataAcquirer : IDataAcquirer
    {
        private readonly RedditContextProvider _redditContextProvider;
        private readonly IEventTracker<RedditDataAcquirer> _eventTracker;

        public RedditDataAcquirer(
            RedditContextProvider redditContextProvider,
            IEventTracker<RedditDataAcquirer> eventTracker)
        {
            _redditContextProvider = redditContextProvider;
            _eventTracker = eventTracker;
        }
        public async IAsyncEnumerable<DataAcquirerPost> GetPostsAsync(
            DataAcquirerInputModel acquirerInputModel)
        {
            var credentials = ExtractCredentials(acquirerInputModel);
            var reddit = await _redditContextProvider.GetContextAsync(credentials);

            // TODO actual searching
            var searchPost = reddit
                .Subreddit("all")
                .Search(new SearchGetSearchInput("Bernie Sanders"));

            var posts = searchPost
                .Select(r => r.Listing)
                .Select(r => DataAcquirerPost.FromValues(
                r.Id,
                r.SelfText,
                "en",
                "reddit",
                "n/a",
                DateTime.Now.ToString("s"),
                acquirerInputModel.Query));

            foreach (var post in posts)
            {
                yield return post;
            }
        }

        private RedditCredentials ExtractCredentials(DataAcquirerInputModel acquirerInputModel)
        {
            return new RedditCredentials(
                acquirerInputModel.Attributes.GetValue("appId"),
                acquirerInputModel.Attributes.GetValue("appSecret"),
                acquirerInputModel.Attributes.GetValue("refreshToken"));
        }
    }



    class Program
    {
        public static void Main(string[] args)
        {
            MainAsync(args).GetAwaiter().GetResult();
        }

        static async Task MainAsync(string[] args)
        {
            var services = Configure();

            var credentialsOptions = services
                .GetRequiredService<IOptions<RedditCredentialsOptions>>();
            var credentials = credentialsOptions.Value;

            var redditAcquirer = services.GetRequiredService<IDataAcquirer>();

            var attributesDict = new Dictionary<string, string>()
            {
                {"appId", credentials.AppId},
                {"appSecret", credentials.AppSecret},
                {"refreshToken", credentials.RefreshToken }
            };

            var attributes = new DataAcquirerAttributes(attributesDict);
            var query = "snake bites NOT piercing NOT darts NOT music";
            var jobId = Guid.NewGuid();
            var inputModel = new DataAcquirerInputModel(
                jobId,
                query,
                null,
                attributes,
                0,
                ulong.MaxValue,
                10);

            var batch = redditAcquirer.GetPostsAsync(inputModel);
            await foreach (var item in batch)
            {
                Console.WriteLine(JsonConvert.SerializeObject(item, Formatting.Indented));
            }
        }
        static async Task MainAsync_2(string[] args)
        {
            // 1) Invoke-WebRequest  "https://www.reddit.com/api/v1/authorize?client_id=Mx2Rp1J2roDMdg&response_type=code&state=wtfisthis&redirect_uri=http://localhost:8080&duration=permanent&scope=read"
            // the same url with sanitizek uri https://www.reddit.com/api/v1/authorize?client_id=Mx2Rp1J2roDMdg&response_type=code&state=wtfisthis&redirect_uri=http%3A%2F%2Flocalhost%3A8080&duration=permanent&scope=read


            // 2) 
            /*$user = 'Mx2Rp1J2roDMdg'
             * $pass = 'eDT3-0no1WHyTuBWTLoNDQNUqWA'
             * $pair = "$($user):$($pass)"
            $encodedCreds = [System.Convert]::ToBase64String([System.Text.Encoding]::ASCII.GetBytes($pair))
>>
>> $basicAuthValue = "Basic $encodedCreds"
>>
>> $Headers = @{
>> Authorization = $basicAuthValue
>> }
            $body = 'grant_type=authorization_code&code=LTPtJwE4rxG7B-hczvtoktPo91A&redirect_uri=http://localhost:8080'
            Invoke-WebRequest -Method Post -Body $body -Headers $Headers "https://www.reddit.com/api/v1/access_token"


    */

            //"access_token": "291925913345-hgvdLNp1OkO0RnkB0vCRiGhuTfk", "token_type": "bearer", "expires_in":
            //        3600, "refresh_token": "291925913345-DFbyOHX5f6zz-__Dqbr41jCOoPs", "scope": "read"}


            // More info here https://github.com/reddit-archive/reddit/wiki/OAuth2
            var reddit = new RedditAPI(
                appId: "Mx2Rp1J2roDMdg",
                appSecret: "eDT3-0no1WHyTuBWTLoNDQNUqWA",
                refreshToken: "291925913345-DFbyOHX5f6zz-__Dqbr41jCOoPs");


            // Since we only need the posts, there's no need to call .About() on this one.  --Kris
            var worldnews = reddit.Subreddit("college");

            // Just keep going until we hit a post from before today.  Note that the API may sometimes return posts slightly out of order.  --Kris
            var posts = new List<SelfPost>();
            string after = "";
            DateTime start = DateTime.Now;
            DateTime today = DateTime.Today.AddDays(-5);
            bool outdated = false;
            var cts = new CancellationTokenSource();
            cts.CancelAfter(TimeSpan.FromMinutes(6));
            do
            {
                foreach (var post in worldnews.Posts.GetNew())
                {
                    if (post.Created >= today)
                    {
                        if (post is SelfPost sp)
                        {
                            posts.Add(sp);
                        }
                    }
                    else
                    {
                        outdated = true;
                        break;
                    }

                    after = post.Fullname;
                }
                Console.WriteLine($"Posts {posts.Count}");
                //if(cts.IsCancellationRequested)
                {
                    break;
                }
            } while (!outdated
                && start.AddMinutes(5) > DateTime.Now
                && worldnews.Posts.New.Count > 0);  // This is automatically populated with the results of the last GetNew call.  --Kris
            Console.ReadLine();
            var x = posts.Take(5).Select(r => r.SelfText);




            //var builtProvider = new DataAcquisitionConsoleAppBuilder()
            //    .AddTransientService<IDataAcquirer, RedditDataAcquirer>()
            //    .ConfigureSpecificOptions<RedditCredentialsOptions>($"Reddit:Credentials")
            //    .Build();


            //var jobManager = builtProvider.GetRequiredService<IJobManager>();

            //var twitterCredentialsOptions = builtProvider.GetService<IOptions<RedditCredentialsOptions>>();


            //var jobConfig = new DataAcquirerJobConfig()
            //{
            //    //Attributes = new Dictionary<string, string>
            //    //{
            //    //    {"TopicQuery", "capi hnizdo" },
            //    //    {"AccessToken", twitterCredentialsOptions.Value.AccessToken},
            //    //    {"AccessTokenSecret" , twitterCredentialsOptions.Value.AccessTokenSecret},
            //    //    {"ApiKey",  twitterCredentialsOptions.Value.ApiKey},
            //    //    {"ApiSecretKey", twitterCredentialsOptions.Value.ApiSecretKey},
            //    //},
            //    JobId = Guid.NewGuid(),
            //    OutputMessageBrokerChannels = new string[] { "MOCK-Post-output" }
            //};
            //try
            //{
            //    await jobManager.StartNewJobAsync(jobConfig);
            //}
            //catch
            //{

            //}

            //await Task.Delay(TimeSpan.FromHours(1));
        }

        public static IServiceProvider Configure()
        {
            var builder = new ConfigurationBuilder()
                           .SetBasePath(Directory.GetCurrentDirectory())
                           .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
            var aspNetCoreEnv = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            if (aspNetCoreEnv == "Development")
            {
                builder.AddJsonFile($"appsettings.Development.json", true, true);
            }
            var configuration = builder.Build();


            var services = new ServiceCollection();
            services.AddLogging(
                logging => logging
                .AddConsole()
                .SetMinimumLevel(LogLevel.Information));

            services.AddSingleton<JobConfigurationUpdateListener>();
            services.AddHostedService<JobConfigurationUpdateListenerHostedService>();

            services.AddTransient<IJobManager, JobManager>();

            services.AddTransient<IRegistrationService, RegistrationService>();



            services.AddSingleton<RedditContextProvider>();

            services.AddSingleton<JobConfigurationUpdateListenerHostedService>();

            services.AddTransient<IMessageBrokerProducer, KafkaProducer>();
            services.AddSingleton<IMessageBrokerConsumer, MockConsumer>();


            services.AddSingleton<IDataAcquirerJobStorage, DataAcquirerJobFileStorage>();
            services.AddSingleton(typeof(IEventTracker<>), typeof(NullEventTracker<>));
            // reddit
            services.AddTransient<IDataAcquirer, RedditDataAcquirer>();

            var rootName = "DataAcquisitionService";

            services.AddOptions<ComponentOptions>()
                .Bind(configuration.GetSection($"{rootName}:ComponentOptions"))
                .ValidateDataAnnotations();

            services.AddOptions<RegistrationRequestOptions>()
                .Bind(configuration.GetSection($"{rootName}:RegistrationRequestOptions"))
                .ValidateDataAnnotations();

            services.AddOptions<KafkaOptions>()
                .Bind(configuration.GetSection($"{rootName}:KafkaOptions"))
                .ValidateDataAnnotations();

            services.AddOptions<RedditCredentialsOptions>()
                .Bind(configuration.GetSection($"Reddit:Credentials"))
                .ValidateDataAnnotations();

            // TW

            //var assemblyPath = (new Uri(Assembly.GetExecutingAssembly().CodeBase)).AbsolutePath;
            //var directory = new FileInfo(assemblyPath).Directory.FullName;
            //var twitterMetaDir = Path.Combine(directory, "metatw");
            //var jobMetaDir = Path.Combine(directory, "metajob");

            //Directory.CreateDirectory(twitterMetaDir);
            //Directory.CreateDirectory(jobMetaDir);

            //services.AddOptions<TwitterJsonStorageOptions>()
            //    .Bind(configuration.GetSection($"{rootName}:TwitterJsonStorageOptions"))
            //    .PostConfigure(o => o.Directory = twitterMetaDir);

            //services.AddOptions<DataAcquirerJobFileStorageOptions>()
            //    .Bind(configuration.GetSection($"{rootName}:DataAcquirerJobFileStorageOptions"))
            //    .PostConfigure(o => o.Directory = jobMetaDir);
            return services.BuildServiceProvider();

        }
    }
}
