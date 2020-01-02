using System;
using Domain;
using Infrastructure.CustomStaticData.MappingAttributes;
using Infrastructure.CustomStaticData.StreamReaders;
using Newtonsoft.Json;

namespace Infrastructure.CustomStaticData
{
    public class CustomStreamReaderFactory
    {
        private readonly IEventTracker<CustomStreamReaderFactory> _eventTracker;
        private readonly IEventTracker<JsonStreamReader> _jsonEventTracker;
        private readonly IEventTracker<CsvStreamReader> _csvEventTracker;

        public CustomStreamReaderFactory(
            IEventTracker<CustomStreamReaderFactory> eventTracker,
            IEventTracker<JsonStreamReader> jsonEventTracker,
            IEventTracker<CsvStreamReader> csvEventTracker)
        {
            _eventTracker = eventTracker;
            _jsonEventTracker = jsonEventTracker;
            _csvEventTracker = csvEventTracker;
        }
        public ICustomStreamReader Create(MappingAttributesRoot attributes)
        {
            try
            {
                if (attributes.DataFormat == "csv")
                {
                    var csvAttributes = attributes.MappingAttributes.ToObject<CsvMappingAttributes>();
                    return new CsvStreamReader(csvAttributes, _csvEventTracker);
                }
                else if (attributes.DataFormat == "json")
                {
                    var jsonAttributes = attributes.MappingAttributes.ToObject<JsonMappingAttributes>();
                    return new JsonStreamReader(jsonAttributes, _jsonEventTracker);
                }
                else
                {
                    var message = $"Unsupported data format '{attributes.DataFormat}'";
                    throw new InvalidOperationException(message);
                }
            }
            catch (JsonException)
            {
                throw;
            }

        }

    }
}
