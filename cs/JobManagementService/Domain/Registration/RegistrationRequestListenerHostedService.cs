using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Domain.Registration
{
    public class RegistrationRequestListenerHostedService : IHostedService
    {
        private readonly RegistrationRequestListener _registrationRequestListener;
        private readonly ILogger<RegistrationRequestListenerHostedService> _logger;
        private Task _listenTask;
        private readonly CancellationTokenSource _cancellationTokenSource 
            = new CancellationTokenSource();

        public RegistrationRequestListenerHostedService(
            RegistrationRequestListener registrationRequestListener,
            ILogger<RegistrationRequestListenerHostedService> logger)
        {
            _registrationRequestListener = registrationRequestListener;
            _logger = logger;
        }
        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Service {serviceName} started", 
                nameof(RegistrationRequestListenerHostedService));
            _listenTask = _registrationRequestListener.Listen(_cancellationTokenSource.Token);
            return Task.CompletedTask;
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Service {serviceName} is stopping",
                nameof(RegistrationRequestListenerHostedService));

            _cancellationTokenSource.Cancel();
            await _listenTask;

            _logger.LogInformation("Service {serviceName} stopped",
                nameof(RegistrationRequestListenerHostedService));
        }
    }
}