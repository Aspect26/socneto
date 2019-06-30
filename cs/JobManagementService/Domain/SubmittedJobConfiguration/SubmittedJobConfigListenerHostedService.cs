using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Domain.ComponentManagement;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Domain.SubmittedJobConfiguration
{
    public class SubmittedJobConfigListenerHostedService:IHostedService
    {
        private readonly SubmittedJobConfigListenerService _submittedJobConfigListenerService;
        private readonly ILogger<SubmittedJobConfigListenerHostedService> _logger;

        private readonly CancellationTokenSource _cancellationTokenSource 
            = new CancellationTokenSource();

        private Task _listeningTask;

        public SubmittedJobConfigListenerHostedService(
            SubmittedJobConfigListenerService submittedJobConfigListenerService,
            ILogger<SubmittedJobConfigListenerHostedService> logger
            )
        {
            _submittedJobConfigListenerService = submittedJobConfigListenerService;
            _logger = logger;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            var token = _cancellationTokenSource.Token;

            _listeningTask = _submittedJobConfigListenerService.Listen(token);

            _logger.LogInformation("Service {serviceName} is started",
                nameof(SubmittedJobConfigListenerHostedService));
            return Task.CompletedTask;
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            _cancellationTokenSource.Cancel();

            _logger.LogInformation("Service {serviceName} is stopping",
                nameof(SubmittedJobConfigListenerHostedService));

            _cancellationTokenSource.Cancel();
            await _listeningTask;

            _logger.LogInformation("Service {serviceName} stopped",
                nameof(SubmittedJobConfigListenerHostedService));
        }
    }

    
}
