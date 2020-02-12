using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

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

            var jArray = JArray.Load(reader);
            var result = new List<Tuple<T1, T2>>();

            if (jArray.Count == 0) return result;

            foreach (var childArray in jArray.Children())
            {
                if (childArray.Type != JTokenType.Array)
                {
                    throw new JsonSerializationException("One of the children is not an array");
                }
                
                var childArrayTyped = childArray as JArray;
                
                if (childArrayTyped.Count != 2)
                    throw new JsonSerializationException("Cannot deserialize array of length different than 2 into tuple");

                result.Add(new Tuple<T1, T2>(childArrayTyped[0].ToObject<T1>(), childArrayTyped[1].ToObject<T2>()));
            }

            return result;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            serializer.Serialize(writer, value);
        }
    }

}