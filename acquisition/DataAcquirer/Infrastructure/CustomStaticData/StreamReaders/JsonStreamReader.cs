using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Domain;
using Domain.Model;
using Infrastructure.CustomStaticData.MappingAttributes;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Infrastructure.CustomStaticData.StreamReaders
{
    public class JsonStreamReader : ICustomStreamReader
    {
        private readonly JsonMappingAttributes _attributes;
        private readonly IEventTracker<JsonStreamReader> _eventTracker;

        private readonly ConcurrentQueue<DataAcquirerPost>
            _posts = new ConcurrentQueue<DataAcquirerPost>();

        public JsonStreamReader(
            JsonMappingAttributes attributes,
            IEventTracker<JsonStreamReader> eventTracker)
        {
            _attributes = attributes;
            _eventTracker = eventTracker;
        }
        public bool ReadingEnded { get; private set; }

        public void StartPopulating(Stream stream)
        {
            try
            {
                using var reader = new StreamReader(stream);
                
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    var post = ParsePostFromLine(line);

                    while (_posts.Count > 1000)
                    {
                        // TODO God forgive me for I have sinned
                        // If anyone know how to implement backpressure,
                        // please let me know
                        Task.Delay(10).GetAwaiter().GetResult();
                    }
                    _posts.Enqueue(post);
                }
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                ReadingEnded = true;
            }
        }

        private DataAcquirerPost ParsePostFromLine(string line)
        {
            var settings = new JsonSerializerSettings()
            {
                DateParseHandling = DateParseHandling.None
            };
            var readPost = JsonConvert.DeserializeObject<JObject>(line, settings);

            var mutablePost = new JObject();
            mutablePost.TryAdd("source", "CustomStaticData");
            foreach (var (k, v) in _attributes.FixedValues)
            {
                mutablePost.TryAdd(k, v);
            }
            foreach (var (k, element) in _attributes.Elements)
            {
                if (readPost.TryGetValue(element, out var value))
                {
                    if (k == "dateTime")
                    {
                        if (DateTime.TryParseExact(
                            value.Value<string>(),
                            _attributes.DateTimeFormatString,
                            null,
                            System.Globalization.DateTimeStyles.None,
                            out var exactTime))
                        {
                            value = exactTime.ToString("s");
                        }
                        else
                        {
                            value = DateTime.Now.ToString("s");
                        }
                    }

                    mutablePost.TryAdd(k, value);
                }
            }
            var post = mutablePost.ToObject<MutableDataAcquirerPost>();
            var dataAcquirerPost = post.Freeze();
            return dataAcquirerPost;
        }

        public bool TryGetPost(out DataAcquirerPost post)
        {
            if (ReadingEnded)
            {
                post = null;
                return false;
            }
            return _posts.TryDequeue(out post);
        }
    }
}
