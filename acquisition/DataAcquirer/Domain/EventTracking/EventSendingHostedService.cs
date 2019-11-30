using System;
using System.Threading;
using System.Threading.Tasks;
using Domain;
using Domain.Abstract;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

public class EventSendingHostedService : IHostedService
{
    private readonly SystemMetricsOptions _systemMetricsOptions;
    private readonly IMessageBrokerProducer _messageBrokerProducer;
    private readonly EventQueue _eventQueue;
    private readonly ILogger<EventSendingHostedService> _logger;
    private Task _loggingTask;

    private readonly CancellationTokenSource _cancellationTokenSource
        = new CancellationTokenSource();
    public EventSendingHostedService(
        IMessageBrokerProducer messageBrokerProducer,
        EventQueue eventQueue,
        IOptions<SystemMetricsOptions> options,
        ILogger<EventSendingHostedService> logger)
    {
        _systemMetricsOptions = options.Value;
        _messageBrokerProducer = messageBrokerProducer;
        _eventQueue = eventQueue;
        _logger = logger;
    }
    public Task StartAsync(CancellationToken cancellationToken)
    {
        _loggingTask = LogAsync(_cancellationTokenSource.Token);
        _logger.LogInformation("Logging service started");
        return Task.CompletedTask;
    }

    public async Task LogAsync(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            using (var ct = new CancellationTokenSource(TimeSpan.FromSeconds(5)))
            {
                var item = _eventQueue.Dequeue(ct.Token);
                if (item == null)
                {
                    await Task.Delay(TimeSpan.FromSeconds(5));
                }
                else
                {
                    var json = JsonConvert.SerializeObject(item);
                    var message = new MessageBrokerMessage("message-key-1", json);
                    await _messageBrokerProducer.ProduceAsync(
                        _systemMetricsOptions.SystemMetricsChannelName
                        , message);
                }
            }
        }
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Logging service is stopping");
        _cancellationTokenSource.Cancel();
        await _loggingTask;
        _logger.LogInformation("Logging service stopped");
    }


}
