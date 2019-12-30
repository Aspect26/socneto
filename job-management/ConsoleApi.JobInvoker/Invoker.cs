using Api.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApi.JobInvoker
{
    public class Invoker
    {
        private readonly JobInvokerOptions _jobInvokerOptions;
        private readonly HttpClient _httpClient;
        private readonly ILogger<Invoker> _logger;

        public Invoker(
            IOptions<JobInvokerOptions> jobInvokerOptions,
            HttpClient httpClient,
            ILogger<Invoker> logger)
        {
            _jobInvokerOptions = jobInvokerOptions.Value;
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task InvokeCommand(JobSubmitRequest request)
        {
            var contentJson = JsonConvert.SerializeObject(request);
            var stringContent = new StringContent(contentJson, Encoding.UTF8, "application/json");
            using (var r = await _httpClient.PostAsync(_jobInvokerOptions.JobSubmitUri, stringContent))
            {
                
                if(!r.IsSuccessStatusCode)
                {
                    var err = await r.Content.ReadAsStringAsync();
                    _logger.LogError("Error on jms side: {error}", err);
                }
            }
        }
    }

}
