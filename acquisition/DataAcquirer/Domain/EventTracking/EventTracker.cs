using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace Domain
{

    public class NullEventTracker<T> : IEventTracker<T>
    {
        private readonly ILogger<NullEventTracker<T>> _logger;

        public NullEventTracker(ILogger<NullEventTracker<T>> logger)
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
    public class EventTracker<T> : IEventTracker<T>
    {
        private readonly EventQueue _eventQueue;
        private readonly ILogger<T> _logger;
        private readonly string _componentOptionsJson;
        private readonly string _componentId;
        private readonly List<string> _levels = new List<string> {
            "FATAL",
            "ERROR",
            "WARNING",
            "INFO",
            "METRICS"
        };
        private readonly int _defaulLogLevelIndex;

        public EventTracker(
            EventQueue eventQueue,
            ILogger<T> logger,
            IOptions<ComponentOptions> componentOptionsAccessor,
            IOptions<LogLevelOptions> logLevelOptionsAccessor)
        {
            _eventQueue = eventQueue;
            _logger = logger;
            _componentOptionsJson = JsonConvert.SerializeObject(componentOptionsAccessor.Value);
            _componentId = componentOptionsAccessor.Value.ComponentId;

            var defaultLogLevel = logLevelOptionsAccessor.Value.Default ?? LogLevel.Information;

            _defaulLogLevelIndex = 0;
            switch (defaultLogLevel)
            {
                case LogLevel.Trace:
                    _defaulLogLevelIndex = _levels.IndexOf("METRICS");
                    break;
                case LogLevel.Debug:
                    _defaulLogLevelIndex = _levels.IndexOf("METRICS");
                    break;
                case LogLevel.Information:
                    _defaulLogLevelIndex = _levels.IndexOf("INFO");
                    break;
                case LogLevel.Warning:
                    _defaulLogLevelIndex = _levels.IndexOf("WARNING");
                    break;
                case LogLevel.Error:
                    _defaulLogLevelIndex = _levels.IndexOf("ERROR");
                    break;
                case LogLevel.Critical:
                    _defaulLogLevelIndex = _levels.IndexOf("FATAL");
                    break;
                case LogLevel.None:
                    _defaulLogLevelIndex = int.MaxValue;
                    break;
            }
        }
        public void TrackError(string eventName, string message, object serializableAttributes = null)
        {
            Track("ERROR", eventName, message, serializableAttributes);
        }

        public void TrackFatal(string eventName, string message, object serializableAttributes = null)
        {
            Track("FATAL", eventName, message, serializableAttributes);
        }

        public void TrackInfo(string eventName, string message, object serializableAttributes = null)
        {
            Track("INFO", eventName, message, serializableAttributes);
        }

        public void TrackStatistics(string eventName, object serializableAttributes = null)
        {
            Track("METRICS", eventName, "statistics", serializableAttributes);
        }

        public void TrackWarning(string eventName, string message, object serializableAttributes = null)
        {
            Track("WARNING", eventName, message, serializableAttributes);
        }

        private void Track(
            string eventType,
            string eventName,
            string message,
            object serializableAttributes = null)
        {
            var currentLevelIndex = _levels.IndexOf(eventType);
            if (_defaulLogLevelIndex < currentLevelIndex)
            {
                return;
            }

            var loggerMessage = "{eventName}:{message}\nOn component: {siteOptions}";
            object[] paramsObjects;
            if (serializableAttributes != null)
            {
                loggerMessage += "\nAttributes:{attributes}";
                paramsObjects = new object[] {
                    eventName,
                    message,
                    _componentOptionsJson,
                    JsonConvert.SerializeObject(serializableAttributes)};
            }
            else
            {
                paramsObjects = new object[] {
                    eventName,
                    message,
                    _componentOptionsJson};
            }

            switch (eventType)
            {
                case "FATAL":
                    _logger.LogCritical(loggerMessage, paramsObjects);
                    break;
                case "ERROR":
                    _logger.LogError(loggerMessage, paramsObjects);
                    break;
                case "WARN":
                    _logger.LogWarning(loggerMessage, paramsObjects);
                    break;
                case "INFO":
                    _logger.LogInformation(loggerMessage, paramsObjects);
                    break;
                case "METRIC":
                    _logger.LogTrace(loggerMessage, paramsObjects);
                    break;
            }

            var metric = new
            {
                componentId = _componentId,
                eventType,
                eventName,
                message,
                timestamp = DateTime.Now.ToString("s"),
                attributes = serializableAttributes
            };
            _eventQueue.Enqueue(metric);
        }
    }
}
