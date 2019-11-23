using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace Domain
{
    public class LogLevelOptions
    {
        public LogLevel? Default { get; set; }
    }
    public class EventTracker<T> : IEventTracker<T>
    {
        private readonly ILogger<T> _logger;
        private readonly string _componentOptionsJson;


        public EventTracker(ILogger<T> logger,
            IOptions<ComponentOptions> componentOptionsAccessor,
            IOptions<LogLevelOptions> logLevelOptionsAccessor)
        {
            _logger = logger;
            _componentOptionsJson = JsonConvert.SerializeObject( componentOptionsAccessor.Value);
            
            // TODO use it when elastic is ready
            var defaulLogLevel = logLevelOptionsAccessor.Value.Default ?? LogLevel.Information;

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
            var loggerMessage = "{eventName}:{message}\nOn component: {siteOptions}";
            object[] paramsObjects;
            if(serializableAttributes!=null)
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
                    _logger.LogCritical(loggerMessage,paramsObjects);
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
            // todo send to elastic
        }
    }

}
