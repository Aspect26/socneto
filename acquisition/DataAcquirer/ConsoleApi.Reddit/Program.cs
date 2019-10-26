using Application;
using Domain.Acquisition;
using Domain.JobConfiguration;
using Domain.JobManagement;
using Domain.Model;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Reddit;
using Reddit.Controllers;
using Reddit.Inputs;
using Reddit.Inputs.Search;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleApi.Reddit
{
    public class RedditCredentialsOptions
    {

    }

    class Program
    {
        public static void Main(string[] args)
        {
            MainAsync(args).GetAwaiter().GetResult();
        }
        static async Task MainAsync(string[] args)
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
                appId:"Mx2Rp1J2roDMdg", 
                appSecret:"eDT3-0no1WHyTuBWTLoNDQNUqWA", 
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
                foreach (Post post in worldnews.Posts.GetNew())
                {
                    if (post.Created >= today)
                    {
                        if(post is SelfPost sp)
                            posts.Add(sp);
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
    }

    public class RedditDataAcquirer : IDataAcquirer
    {

        public async Task<DataAcquirerOutputModel> AcquireBatchAsync(
            DataAcquirerInputModel acquirerInputModel,
            CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
