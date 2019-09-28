using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using LinqToTwitter;
using Newtonsoft.Json;
using SmokeTester.Model;
using Socneto.DataAcquisition.Infrastructure.Kafka;

namespace SmokeTester
{
    public class SmokeTester
    {
        private readonly CoordinatorClient _client;
        private readonly KafkaConsumer _consumer;

        public SmokeTester(CoordinatorClient client, KafkaConsumer consumer)
        {
            _client = client;
            _consumer = consumer;
        }
        
        public async Task Test()
        {
            await TestSubmit();
            Console.WriteLine("Everything is ok");

        }

        private async Task TestSubmit()
        {
            var cancellationTokenSource = new CancellationTokenSource();
            cancellationTokenSource.CancelAfter(TimeSpan.FromSeconds(10));

            var submitRequest = new SubmitRequest()
            {
                Query = $"FooBar-{DateTime.Now:s}"
            };
            var json =JsonConvert.SerializeObject(submitRequest);

            Console.WriteLine($"Request: {json}");
            Guid jobId = Guid.Empty;

            var recieved = new List<string>();
            
            Console.WriteLine($"Posting {json}");
            try
            {
                var responseBody = await _client.Post(json);
                var expectedResponse = JsonConvert.DeserializeObject< SubmitExpectedResponse>(responseBody);
                if (expectedResponse is null)
                {
                    throw new SmokeTesterException("api/submit", "Deserialized response is not correct");
                }

                jobId = expectedResponse.JobId;
                Console.WriteLine($"Response: {responseBody}");
            }
            catch (HttpRequestException e)
            {
                throw new SmokeTesterException("api/submit","Http error! See inner exception", e);

            }

            await _consumer.ConsumeAsync(
                message =>
                {
                    try
                    {
                        recieved.Add(message);
                        Console.WriteLine($"Recieved: {message}");

                    }
                    catch (Exception e)
                    {
                        throw new SmokeTesterException("submit-produced", $"Recieve error of {message}", e);
                    }

                    return Task.CompletedTask;
                }
                , cancellationTokenSource.Token);

            if (recieved.Count == 0)
            {
                throw new SmokeTesterException("submit-produced", "No message recieved");
            }

            //if (recieved.Count > 1)
            //{
            //    throw new SmokeTesterException("submit-produced", "Too many object recieved");
            //}

            // take the last one only
            var recievedJson = recieved[recieved.Count - 1];

            var deserialized = JsonConvert.DeserializeObject<DataAcquisitionRequestMessage>(recievedJson);
            if (jobId != deserialized.JobId)
            {
                throw new SmokeTesterException("submit-produced"
                    , $"Deserialized object does not correspond to the sent one {jobId}-{deserialized.JobId}");
            }
            if (submitRequest.Query != deserialized.Query)
            {
                throw new SmokeTesterException("submit-produced"
                    , $"Deserialized object does not correspond to the sent one {submitRequest.Query}-{deserialized.Query}");
            }


        }


        
    }
}