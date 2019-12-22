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

        private readonly List<string> _levels = new List<string> {
            "fatal",
            "error",
            "warning",
            "info",
            "metrics"
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

            var defaultLogLevel = logLevelOptionsAccessor.Value.Default ?? LogLevel.Information;

            _defaulLogLevelIndex = 0;
            switch (defaultLogLevel)
            {
                case LogLevel.Trace:
                    _defaulLogLevelIndex = _levels.IndexOf("metrics");
                    break;
                case LogLevel.Debug:
                    _defaulLogLevelIndex = _levels.IndexOf("metrics");
                    break;
                case LogLevel.Information:
                    _defaulLogLevelIndex = _levels.IndexOf("info");
                    break;
                case LogLevel.Warning:
                    _defaulLogLevelIndex = _levels.IndexOf("warning");
                    break;
                case LogLevel.Error:
                    _defaulLogLevelIndex = _levels.IndexOf("error");
                    break;
                case LogLevel.Critical:
                    _defaulLogLevelIndex = _levels.IndexOf("fatal");
                    break;
                case LogLevel.None:
                    _defaulLogLevelIndex = int.MaxValue;
                    break;
            }
        }
        public void TrackError(string eventName, string message, object serializableAttributes = null)
        {
            Track("error", eventName, message, serializableAttributes);
        }

        public void TrackFatal(string eventName, string message, object serializableAttributes = null)
        {
            Track("fatal", eventName, message, serializableAttributes);
        }

        public void TrackInfo(string eventName, string message, object serializableAttributes = null)
        {
            Track("info", eventName, message, serializableAttributes);
        }

        public void TrackStatistics(string eventName, object serializableAttributes = null)
        {
            Track("metrics", eventName, "statistics", serializableAttributes);
        }

        public void TrackWarning(string eventName, string message, object serializableAttributes = null)
        {
            Track("warning", eventName, message, serializableAttributes);
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
                case "fatal":
                    _logger.LogCritical(loggerMessage, paramsObjects);
                    break;
                case "error":
                    _logger.LogError(loggerMessage, paramsObjects);
                    break;
                case "warning":
                    _logger.LogWarning(loggerMessage, paramsObjects);
                    break;
                case "info":
                    _logger.LogInformation(loggerMessage, paramsObjects);
                    break;
                case "metrics":
                    _logger.LogTrace(loggerMessage, paramsObjects);
                    break;
            }

            var metric = new
            {
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
