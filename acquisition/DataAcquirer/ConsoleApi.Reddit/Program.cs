using Application;
using Domain;
using Domain.Abstract;
using Domain.Acquisition;
using Domain.EventTracking;
using Domain.JobConfiguration;
using Domain.JobManagement;
using Domain.JobManagement.Abstract;
using Domain.Model;
using Domain.Registration;
using Infrastructure.DataGenerator;
using Infrastructure.Kafka;
using Infrastructure.Reddit;
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
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleApi.Reddit
{


    class Program
    {
        public static async Task Main(string[] args)
        {
            //MainAsync_2(args)
            //TestRedditAsync().GetAwaiter().GetResult();
            await MainAsync(args);
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
                3);

            var batch = redditAcquirer.GetPostsAsync(inputModel);
            await foreach (var item in batch)
            {
                Console.WriteLine(JsonConvert.SerializeObject(item, Formatting.Indented));
            }
            Console.WriteLine("Search ended");
            Console.ReadLine();
        }
        static async Task MainAsync_2(string[] args)
        {
            // `between` and `after` does not work together
            var query = "\"new year\"";
            var posts = GetPosts(query, limit: 25);
            var tenth = posts[9].Fullname;
            var fifteenth = posts[15].Fullname;

            var afterTenth = GetPosts(query, after: tenth, limit: 25);
            var beforeFifteenth = GetPosts(query, before: fifteenth, limit: 25);
            var betweenTenAndFifteen = GetPosts(query, after: tenth, before: fifteenth, limit: 25, count: 0);
            Console.WriteLine("{0};{1}", tenth, fifteenth);
            foreach (var post in posts)
            {
                Console.WriteLine(post.Fullname);
            }

            Console.WriteLine("Data");
            foreach (var post in afterTenth)
            {
                Console.WriteLine(post.Fullname);
            }
            Console.WriteLine("Data");
            foreach (var post in beforeFifteenth)
            {
                Console.WriteLine(post.Fullname);
            }
            Console.WriteLine("Data");
            foreach (var post in betweenTenAndFifteen)
            {
                Console.WriteLine(post.Fullname);
            }


            Console.ReadLine();

            //TestReddit();

            //// Just keep going until we hit a post from before today.  Note that the API may sometimes return posts slightly out of order.  --Kris
            //var posts = new List<SelfPost>();
            //string after = "";
            //DateTime start = DateTime.Now;
            //DateTime today = DateTime.Today.AddDays(-5);
            //bool outdated = false;
            //var cts = new CancellationTokenSource();
            //cts.CancelAfter(TimeSpan.FromMinutes(6));
            //do
            //{
            //    foreach (var post in worldnews.Posts.GetNew())
            //    {
            //        if (post.Created >= today)
            //        {
            //            if (post is SelfPost sp)
            //            {
            //                posts.Add(sp);
            //            }
            //        }
            //        else
            //        {
            //            outdated = true;
            //            break;
            //        }

            //        after = post.Fullname;
            //    }
            //    Console.WriteLine($"Posts {posts.Count}");
            //    //if(cts.IsCancellationRequested)
            //    {
            //        break;
            //    }
            //} while (!outdated
            //    && start.AddMinutes(5) > DateTime.Now
            //    && worldnews.Posts.New.Count > 0);  // This is automatically populated with the results of the last GetNew call.  --Kris
            //Console.ReadLine();
            //var x = posts.Take(5).Select(r => r.SelfText);




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

            await Task.Delay(TimeSpan.FromHours(1));

        }

        private static async Task TestRedditAsync()
        {
            var reddit = new RedditClient(
                appId: "Mx2Rp1J2roDMdg",
                appSecret: "eDT3-0no1WHyTuBWTLoNDQNUqWA",
                refreshToken: "291925913345-DFbyOHX5f6zz-__Dqbr41jCOoPs");
            var indices = Enumerable.Range(0, 100);
            // ei33zr
            // ei37wh
            string after = null;
            var limit = 50;
            //var before = "ei2mja";
            DateTime? before = null;
            var query = "university OR study OR studying OR college NOT football";
            // Since we only need the posts, there's no need to call .About() on this one.  --Kris
            int total = 0;
            //string MaxStr(string a, string b)
            //{
            //    var cmp = StringComparer.Create(CultureInfo.InvariantCulture, false);

            //    if (cmp.Compare(a, b) > 0)
            //    {
            //        return a;
            //    }
            //    return b;
            //}

            DateTime Max(DateTime a, DateTime? b)
            {
                if (!b.HasValue)
                {
                    return a;
                }

                if (a < b.Value)
                {
                    return b.Value;
                }
                return a;
            }
            List<Post> get(
                RedditClient reddit,
                string after,
                int limit,
                string query,
                int count)
            {
                var searchInput = new SearchGetSearchInput(
                                        q: query,
                                        after: after,
                                        //  before:before,
                                        limit: limit,
                                        count: count);
                return reddit.Search(searchInput);
            }
            var subs = new ConcurrentDictionary<string, int>();
            while (true)
            {
                var maxBefore = before;
                var count = 0;
                var postListing = get(reddit, after, limit, query, count);
                var outDated = false;
                while (postListing.Count > 0)
                {
                    var children = postListing;
                    foreach (var item in children)
                    {
                        if (item.Created <= before)
                        {
                            outDated = true;
                            Console.WriteLine("Outdated encountered");
                            break;
                        }
                        
                        count++;
                        subs.AddOrUpdate(item.Subreddit, 1, (k, v) => v + 1);
                        maxBefore = Max(item.Created, maxBefore);
                        var title = item.Title;
                        var text = item.Listing.SelfText;
                        var subLength = 20000;
                        if (text.Length > subLength)
                        {
                            text = text.Substring(0, subLength);
                        }
                        Console.WriteLine($"{item.Fullname} {item.Listing.CreatedUTC}");
                        Console.WriteLine($"\t{title}");
                        Console.WriteLine($"\t{text}");
                        var comments = item.Comments.GetTop(100);
                        foreach (var (i, c) in indices.Zip(comments))
                        {
                            Console.WriteLine($"C-{$"{i:00}"}:\t{c.Body}");
                        }
                        Console.WriteLine(string.Concat(Enumerable.Range(0, 80).Select(r => "*")));


                    }
                    PrintDict(subs, total + count);
                    if (outDated)
                    {
                        Console.WriteLine("outdated");
                        break;
                    }
                    after = postListing.Count > 0 ? postListing.Last().Fullname : after;
                    Console.WriteLine($"after:{after}");
                    postListing = get(reddit, after, limit, query, count);
                }
                before = maxBefore;
                Console.WriteLine($"waiting: before; {before} after: {after}, c:{count} ");
                total += count;

                PrintDict(subs, total);
                after = null;
                count = 0;
                await Task.Delay(TimeSpan.FromSeconds(10));
            }
        }

        private static void PrintDict(ConcurrentDictionary<string, int> subs, int total)
        {
            Console.WriteLine($"total:{total}");
            foreach (var (k, v) in subs
                                .OrderByDescending(r => r.Value)
                                .Take(10))
            {
                Console.WriteLine($"\t{k}:{v}");
            }
            if (subs.TryGetValue("ApplyingToCollege", out var val))
            {
                Console.WriteLine($"applying: {val}");
            }
        }


        private static List<Post> GetPosts(
            string query,
            string after = null,
            string before = null,
            int limit = 25,
            int count = 0)
        {
            var reddit = new RedditClient(
                            appId: "Mx2Rp1J2roDMdg",
                            appSecret: "eDT3-0no1WHyTuBWTLoNDQNUqWA",
                            refreshToken: "291925913345-DFbyOHX5f6zz-__Dqbr41jCOoPs");

            var searchInput = new SearchGetSearchInput(
                q: query,
                after: after,
                before: before,
                limit: limit,
                count: count);
            return reddit.Search(searchInput);
        }

        public static IServiceProvider Configure()
        {
            var builder = new ConfigurationBuilder()
                           .SetBasePath(Directory.GetCurrentDirectory())
                           .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
#if DEBUG
            builder.AddJsonFile($"appsettings.Development.json", true, true);
#endif
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
