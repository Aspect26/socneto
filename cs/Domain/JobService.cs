using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Socneto.Domain.Models;

namespace Socneto.Domain
{

    public class JobService : IJobService
    {

        private readonly IResultProducer _producer;
        private readonly ILogger<JobService> _logger;


        public JobService(IResultProducer producer, ILogger<JobService> logger)
        {
            _producer = producer;
            _logger = logger;
        }


        public async Task<JobSubmitResult> SubmitJob(JobSubmitInput jobInput)
        {
            var guid = Guid.NewGuid();

            _logger.LogInformation("Processing data");


            var job = new JobDataRequest()
            {
                JobId = guid,
                Query = jobInput.Query
            };

            var json = JsonConvert.SerializeObject(job);


            await _producer.ProduceAsync(
                new Models.Message()
                {
                    // this is not required
                    Key = "Request",
                    Value = json
                }
            );

            return new JobSubmitResult() { JobId = guid };
        }

    }
}
