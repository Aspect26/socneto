using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Domain.JobConfiguration
{
    public class JobConfigurationUpdateListenerHostedService : IHostedService
    {
        private readonly JobConfigurationUpdateListener _jobConfigurationUpdateListener;
        private readonly ILogger<JobConfigurationUpdateListenerHostedService> _logger;
        private readonly CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
        private Task _listenTask;

        public JobConfigurationUpdateListenerHostedService(
            JobConfigurationUpdateListener jobConfigurationUpdateListener,
            ILogger<JobConfigurationUpdateListenerHostedService> logger)
        {
            _jobConfigurationUpdateListener = jobConfigurationUpdateListener;
            _logger = logger;
        }
        
        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Service {serviceName} started",
                nameof(JobConfigurationUpdateListenerHostedService));

            _listenTask = Task.Run(async () => await _jobConfigurationUpdateListener.ListenAsync(_cancellationTokenSource.Token), cancellationToken);
            return Task.CompletedTask;
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Service {serviceName} is stopping",
                nameof(JobConfigurationUpdateListenerHostedService));

            _cancellationTokenSource.Cancel();
            try
            {
                await _listenTask;
            }
            catch (TaskCanceledException) { }

            _logger.LogInformation("Service {serviceName} stopped",
                nameof(JobConfigurationUpdateListenerHostedService));
        }
    }
}
