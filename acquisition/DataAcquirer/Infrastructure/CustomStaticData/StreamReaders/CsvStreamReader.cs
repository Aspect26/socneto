using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using CsvHelper;
using Domain;
using Domain.Model;
using Infrastructure.CustomStaticData.MappingAttributes;
using Newtonsoft.Json.Linq;

namespace Infrastructure.CustomStaticData.StreamReaders
{
    public class CsvStreamReader : ICustomStreamReader
    {
        private readonly ConcurrentQueue<DataAcquirerPost>
            _posts = new ConcurrentQueue<DataAcquirerPost>();

        private readonly CsvMappingAttributes _attributes;
        private readonly IEventTracker<CsvStreamReader> _eventTracker;

        public CsvStreamReader(CsvMappingAttributes attributes,
            IEventTracker<CsvStreamReader> eventTracker)
        {
            _attributes = attributes;
            _eventTracker = eventTracker;
        }

        public bool ReadingEnded { get; private set; } = false;

        public bool TryGetPost(out DataAcquirerPost post)
        {
            if (ReadingEnded)
            {
                post = null;
                return false;
            }
            return _posts.TryDequeue(out post);
        }

        public void StartPopulating(Stream stream)
        {
            try
            {
                using var se = new StreamReader(stream);
                using var csvreader = new CsvReader(se);

                if (_attributes.HasHeaders)
                {
                    csvreader.Read();
                }

                while (csvreader.Read())
                {
                    var dataAcquirerPost = ParsePostFromReader(csvreader);

                    while (_posts.Count > 1000)
                    {
                        // TODO God forgive me for I have sinned
                        // If anyone know how to implement backpressure,
                        // please let me know
                        Task.Delay(10).GetAwaiter().GetResult();
                    }
                    _posts.Enqueue(dataAcquirerPost);
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

        private DataAcquirerPost ParsePostFromReader(CsvReader csvreader)
        {
            var mutablePost = new JObject();
            mutablePost.TryAdd("source", "CustomStaticData");
            foreach (var (k, v) in _attributes.FixedValues)
            {
                mutablePost.TryAdd(k, v);
            }
            foreach (var (k, index) in _attributes.Indices)
            {
                if (csvreader.TryGetField<string>(index, out var value))
                {
                    if (k == "dateTime")
                    {
                        if (DateTime.TryParseExact(
                            value,
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
    }
}
