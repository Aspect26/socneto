using System;
using Newtonsoft.Json;

namespace Socneto.Domain.Helpers
{
    public class JsonMillisecondsToDateTimeConverter : Newtonsoft.Json.JsonConverter
    { 
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(DateTime);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var t = (Int64)reader.Value;
            return new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddMilliseconds(t);
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            DateTime typedValue = (DateTime) value;
            writer.WriteValue(typedValue);
        }
    }
}