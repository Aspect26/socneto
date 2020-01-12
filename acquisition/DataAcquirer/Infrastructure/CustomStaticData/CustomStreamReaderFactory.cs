using System;
using Domain;
using Infrastructure.CustomStaticData.MappingAttributes;
using Infrastructure.CustomStaticData.StreamReaders;
using Newtonsoft.Json;

namespace Infrastructure.CustomStaticData
{
    public class CustomStreamReaderFactory
    {
        public ICustomStreamReader Create(MappingAttributesRoot attributes)
        {
            try
            {
                if (attributes.DataFormat == "csv")
                {
                    var csvAttributes = attributes.MappingAttributes.ToObject<CsvMappingAttributes>();
                    return new CsvStreamReader(csvAttributes);
                }
                else if (attributes.DataFormat == "json")
                {
                    var jsonAttributes = attributes.MappingAttributes.ToObject<JsonMappingAttributes>();
                    return new JsonStreamReader(jsonAttributes);
                }
                else
                {
                    var message = $"Unsupported data format '{attributes.DataFormat}'";
                    throw new InvalidOperationException(message);
                }
            }
            catch (JsonException je)
            {
                throw new InvalidOperationException($"Could not parse mapping", je);
            }
        }

    }
}
