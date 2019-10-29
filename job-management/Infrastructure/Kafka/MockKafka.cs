using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Domain;
using Domain.Abstract;
using Domain.Models;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Infrastructure.Kafka
{
    public class MockKafka : IMessageBrokerConsumer,IMessageBrokerProducer
    {
        private readonly ILogger<MockKafka> _logger;

        public MockKafka(ILogger<MockKafka> logger)
        {
            _logger = logger;
        }

        private static Guid _jobId = Guid.NewGuid();
        public async Task ConsumeAsync(string consumeTopic,
            Func<string, Task> onRecieveAction,
            CancellationToken cancellationToken)
        {
            _logger.LogInformation("Topic {}", consumeTopic);


            //if (consumeTopic == "job_management.job_configuration.DataAnalyzer_Mock")
            //{
            //    //var config = new DataAcquirerJobConfig()
            //    //{
            //    //    JobId = Guid.NewGuid(),
            //    //    Attributes = new Dictionary<string, string>
            //    //    {
            //    //        {"TopicQuery","FooBar"}
            //    //    },
            //    //    OutputMessageBrokerChannels = new string[] { "s1", "a1", "a2" }
            //    //};
            //    var config = new DataAnalyzerJobConfig()
            //    {
            //        JobId = _jobId,
            //        OutputChannelName = "Storage_output_channel"
            //    };
            //    var json = JsonConvert.SerializeObject(config);
            //    await onRecieveAction(json);
            //}
            //if (consumeTopic == "job_management.component_data_input.DataAnalyzer_Mock")
            //{
            //    await Task.Delay(TimeSpan.FromSeconds(10));
            //    while (true)
            //    {
            //        await Task.Delay(TimeSpan.FromSeconds(10));

            //        var config = UniPost.FromValues(
            //            Guid.NewGuid().ToString(),
            //            "RANDOM text",
            //        "test souirce",
            //            DateTime.Now.ToString("s"),
            //            "foobarUser",
            //            _jobId
            //        );

            //        var json = JsonConvert.SerializeObject(config);
            //        await onRecieveAction(json);
            //    }
            //}

            if (consumeTopic == "job_management.registration.request")
            {
                await Task.Delay(TimeSpan.FromSeconds(4));
                var registration = new 
                {
                    componentId = "mock",
                    componentType = "DATA_ANALYSER",
                    attributes = new 
                    {
                        outputFormat = new
                        {
                            o1= new
                            {
                                i1 = 1,
                                i2 = 2
                            },
                            o2=new[] {1,2,3,4,5} 
                        }
                    },
                    updateChannelName = "test",
                    inputChannelName = "test"

                };
                var jsonRequres = JsonConvert.SerializeObject(registration);
                
                await onRecieveAction(jsonRequres);
                
            }
            // prevents app from turning off
            await Task.Delay(TimeSpan.FromMinutes(10));
        }
    
        public Task ProduceAsync(string topic, MessageBrokerMessage message)
        {
            _logger.LogInformation("Topic {}, Message {}",
                topic,
                message.JsonPayloadPayload);

            return Task.CompletedTask;
        }
    }
}