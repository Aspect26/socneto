using System;
using System.Net.Sockets;
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

        public async Task<bool> ComponentRunning()
        {
            try
            {
                var hello = await _httpService.Get<JMSHello>("api/test/say-hello");
                return hello.Message == "hello";
            }
            catch (SocketException)
            {
                return false;
            }
        }

        public async Task<JobStatus> SubmitJob(JobSubmit jobSubmit)
        {
            return await _httpService.Post<JobStatus>($"api/job/submit", jobSubmit);
        }

        public async Task<JobStatus> StopJob(Guid jobId)
        {
            return await _httpService.Get<JobStatus>($"api/job/stop/{jobId}");
        }
    }
}