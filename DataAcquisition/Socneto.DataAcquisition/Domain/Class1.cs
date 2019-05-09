﻿using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace Socneto.DataAcquisition.Domain
{
    public class StuffDoer
    {
        private readonly IJobConsumer _consumer;
        private readonly IResultProducer _resultProducer;
        private readonly ILogger<StuffDoer> _logger;

        public StuffDoer(IJobConsumer consumer, IResultProducer resultProducer, ILogger<StuffDoer> logger )
        {
            _consumer = consumer;
            _resultProducer = resultProducer;
            _logger = logger;
        }

        public async Task DoSomeRealWork()
        {
            _logger.LogInformation("Consuming started");
            await _consumer.ConsumeAsync( ProduceRecords, CancellationToken.None );
        }


        private async Task ProduceRecords(string input)
        {
            // TODO i should deeserialize right at the consumer or register deserializer
            var taskInput = JsonConvert.DeserializeObject<TaskInput>(input);

            _logger.LogInformation($"Generating 10 mock records for: '{taskInput.Query}'");
            //... todo

            for (int i = 0; i < 10; i++)
            {
                var text =taskInput.Query +"-"+ RandomString(random.Next(50, 100));
                var user = RandomString(random.Next(5, 15));
                var source = "twitter";

                var post =  UniPost.FromValues(text, source, user);
                var serialized = JsonConvert.SerializeObject(post);
                var message = new Message()
                {
                    Value = serialized,
                    Key = "dunno"
                };

                await _resultProducer.ProduceAsync( message);
            }
        }


        private static Random random = new Random();
        public static string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }

    

    public class TaskInput
    {
        public string Query { get; set; }
    }

    //public interface ISocialNetwork
    //{
    //    Task<IList<UniPost>> SearchAsync(string searchTerm);
    //}
}
