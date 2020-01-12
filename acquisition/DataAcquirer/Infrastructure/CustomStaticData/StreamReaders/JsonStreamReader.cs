using System;
using System.IO;
using Domain;
using Domain.Model;
using Infrastructure.CustomStaticData.MappingAttributes;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Infrastructure.CustomStaticData.StreamReaders
{
    public class JsonStreamReader : BaseStreamReader, ICustomStreamReader
    {
        private readonly JsonMappingAttributes _attributes;
        
        public JsonStreamReader(
            JsonMappingAttributes attributes)
        {
            _attributes = attributes;
        }

        public void StartPopulating(Stream stream)
        {
            try
            {
                using var reader = new StreamReader(stream);

                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    var post = ParsePostFromLine(line);

                    _posts.Add(post);
                }
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                _posts.CompleteAdding();
            }
        }

        private DataAcquirerPost ParsePostFromLine(string line)
        {
            var settings = new JsonSerializerSettings()
            {
                DateParseHandling = DateParseHandling.None
            };
            var readPost = JsonConvert.DeserializeObject<JObject>(line, settings);

            var builder = new PostBuilder(_attributes.DateTimeFormatString)
                .AddSource()
                .PopulateFixed(_attributes.FixedValues);

            foreach (var (k, element) in _attributes.Elements)
            {
                if (readPost.TryGetValue(element, out var valueToken))
                {
                    var value = valueToken.Value<string>();
                    builder.PopulateField(k, value);
                }
            }

            return builder.Build();
        }
    }
}
