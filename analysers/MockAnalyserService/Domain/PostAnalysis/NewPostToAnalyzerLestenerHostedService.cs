using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Domain.PostAnalysis
{
    public class NewPostToAnalyzerLestenerHostedService : IHostedService
    {
        private readonly NewPostToAnalyzeListener _newPostToAnalyzeListener;
        private readonly ILogger<NewPostToAnalyzerLestenerHostedService> _logger;
        private readonly CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
        private Task _listenTask;

        public NewPostToAnalyzerLestenerHostedService(
            NewPostToAnalyzeListener newPostToAnalyzeListener,
            ILogger<NewPostToAnalyzerLestenerHostedService> logger)
        {
            _newPostToAnalyzeListener = newPostToAnalyzeListener;
            _logger = logger;
        }
        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Service {serviceName} started",
                nameof(NewPostToAnalyzerLestenerHostedService));

            _listenTask = _newPostToAnalyzeListener.ListenAsync(_cancellationTokenSource.Token);
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Service {serviceName} is stopping",
                nameof(NewPostToAnalyzerLestenerHostedService));

            _cancellationTokenSource.Cancel();
            try
            {
                await _listenTask;
            }
            catch (TaskCanceledException) { }

            _logger.LogInformation("Service {serviceName} stopped",
                nameof(NewPostToAnalyzerLestenerHostedService));
        }
    }
}
