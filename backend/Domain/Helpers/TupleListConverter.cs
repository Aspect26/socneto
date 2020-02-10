using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Socneto.Domain.Helpers
{
    public class TupleListConverter<T1, T2> : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return typeof(IList<Tuple<T1, T2>>) == objectType;
        }

        public override object ReadJson(
            JsonReader reader,
            Type objectType,
            object existingValue,
            JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null)
                return null;

            var jArray = Newtonsoft.Json.Linq.JArray.Load(reader);
            var result = new List<Tuple<T1, T2>>();
            
            foreach (var childArray in jArray.Children<Newtonsoft.Json.Linq.JArray>())
            {
                if (childArray.Count != 2)
                    throw new JsonSerializationException("Cannot deserialize array of length different than 2 into tuple");
                
                result.Add(new Tuple<T1, T2>(childArray[0].ToObject<T1>(), childArray[1].ToObject<T2>()));
            }

            return result;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            serializer.Serialize(writer, value);
        }
    }

}