using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Socneto.Domain.Models;

namespace Socneto.Domain.Services
{
    public class JobManagementService : IJobManagementService
    {
        
        private readonly HttpService<JobManagementService> _httpService;
        
        public JobManagementService(ILogger<JobManagementService> logger, IOptions<JMSOptions> jmsOptionsObject)
        {
            if (string.IsNullOrEmpty(jmsOptionsObject.Value.ServerAddress))
                throw new ArgumentNullException(nameof(jmsOptionsObject.Value.ServerAddress));
            
            var host = jmsOptionsObject.Value.ServerAddress;
            _httpService = new HttpService<JobManagementService>(host, logger);
        }

        public async Task<JobStatus> SubmitJob()
        {
            throw new System.NotImplementedException();
        }

        public async Task<JobStatus> StopJob(string jobId)
        {
            return await _httpService.Get<JobStatus>($"job/stop/{jobId}");
        }
    }
}