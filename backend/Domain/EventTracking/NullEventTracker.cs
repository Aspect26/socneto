using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Domain.EventTracking
{
    public class ConsoleEventTracker<T> : IEventTracker<T>
    {
        private readonly ILogger<ConsoleEventTracker<T>> _logger;

        public ConsoleEventTracker(ILogger<ConsoleEventTracker<T>> logger)
        {
            _logger = logger;
        }
        public void TrackError(string eventName, string message, object serializableAttributes = null)
        {
            var attributes = serializableAttributes != null
                ? JsonConvert.SerializeObject(serializableAttributes)
                : "none";
            _logger.LogError("{eventName}: {message}. Payload: {payload}",
                eventName,
                message,
                attributes);
        }

        public void TrackFatal(string eventName, string message, object serializableAttributes = null)
        {
            var attributes = serializableAttributes != null
                ? JsonConvert.SerializeObject(serializableAttributes)
                : "none";
            _logger.LogError("{eventName}: {message}. Payload: {payload}",
              eventName,
              message,
              attributes);
        }

        public void TrackInfo(string eventName, string message, object serializableAttributes = null)
        {
            var attributes = serializableAttributes != null
                ? JsonConvert.SerializeObject(serializableAttributes)
                : "none";
            _logger.LogInformation("{eventName}: {message}. Payload: {payload}",
              eventName,
              message,
              attributes);
        }

        public void TrackStatistics(string eventName, object serializableAttributes = null)
        {
            var attributes = serializableAttributes != null
                ? JsonConvert.SerializeObject(serializableAttributes)
                : "none";
            _logger.LogTrace("{eventName}: {message}. Payload: {payload}",
              eventName,
              attributes);
        }

        public void TrackWarning(string eventName, string message, object serializableAttributes = null)
        {
            var attributes = serializableAttributes != null
                ? JsonConvert.SerializeObject(serializableAttributes)
                : "none";
            _logger.LogWarning("{eventName}: {message}. Payload: {payload}",
              eventName,
              message,
              attributes);
        }
    }
}
