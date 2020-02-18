using Domain.DependencyWaiting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace Infrastructure.DependencyWaiting
{
    public class DependencyWaitingService 
    {
        private readonly IStorageDependencyWaitingService _storageDependencyWaitingService;
        private readonly IKafkaDependencyWaitingService _kafkaDependencyWaitingService;
        private readonly ILogger<DependencyWaitingService> _logger;

        public DependencyWaitingService(
            IStorageDependencyWaitingService storageDependencyWaitingService,
            IKafkaDependencyWaitingService kafkaDependencyWaitingService,
            ILogger<DependencyWaitingService> logger)
        {
            _storageDependencyWaitingService = storageDependencyWaitingService;
            _kafkaDependencyWaitingService = kafkaDependencyWaitingService;
            _logger = logger;
        }
        public async Task<bool> AreDependenciesReady()
        {
            var storageReadyTask = _storageDependencyWaitingService.IsDependencyReadyAsync();
            var kafkaReadyTask = _kafkaDependencyWaitingService.IsDependencyReadyAsync();
            try
            {
                await Task.WhenAll(storageReadyTask, kafkaReadyTask);
                return storageReadyTask.Result && kafkaReadyTask.Result;
            }
            catch (Exception e)
            {
                _logger.LogError("Dependencies waiting failed: {exception}", e);
                return false;
            }
        }
    }
}
