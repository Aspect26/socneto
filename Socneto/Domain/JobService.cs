using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Socneto.Coordinator.Domain.Interfaces;
using Socneto.Coordinator.Domain.Models;

namespace Socneto.Coordinator.Domain
{
    
    public class JobService : IJobService
    {
        private readonly IDataCollector _dataCollector;
        
        private readonly IResultProducer _producer;
        private readonly ILogger<JobService> _logger;
        
        
        public JobService(IDataCollector dataCollector, IResultProducer producer, ILogger<JobService> logger)
        {
            _dataCollector = dataCollector;
            
            _producer = producer;
            _logger = logger;
        }


        public async Task<JobSubmitResult> SubmitJob(JobSubmitInput jobInput)
        {
            var page = await _dataCollector.CollectDataAsync(jobInput);
            var guid = Guid.NewGuid().ToString();

            _logger.LogInformation("Processing data");

            foreach (var post in page.PostDataList)
            {
                var job = new JobResultWrapper()
                {
                    JobId = guid,
                    Data = post,
                    DataType = "post"
                };
                
                var json = JsonConvert.SerializeObject(post);


                await _producer.ProduceAsync(
                    new Message()
                    {
                        Key = "Post",
                        Value = json
                    }
                );
            }
            return new JobSubmitResult() { JobId = guid };
        }

    }
}
