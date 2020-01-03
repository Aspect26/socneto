using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Domain.Exceptions;
using Domain.Model;
using Newtonsoft.Json.Linq;

namespace Infrastructure.CustomStaticData.StreamReaders
{
    public class PostBuilder
    {
        private readonly JObject _post = new JObject();
        private readonly string _dateTimeString;
        private const string _sourceFieldName = "source";
        
        public PostBuilder(string dateTimeString = null)
        {
            _dateTimeString = dateTimeString;
        }
        public PostBuilder AddSource()
        {
            _post.TryAdd(_sourceFieldName, "CustomStaticData");
            ;
            return this;
        }
        public PostBuilder PopulateFixed(IDictionary<string, string> fixedValues)
        {
            foreach (var (k, v) in fixedValues)
            {
                _post.TryAdd(k, v);
            }
            return this;
        }

        public DataAcquirerPost Build()
        {
            var post = _post.ToObject<MutableDataAcquirerPost>();
            return post.Freeze();
        }

        public PostBuilder PopulateField(string fieldName, string value)
        {
            if (fieldName != "dateTime")
            {
                _post.TryAdd(fieldName, value);
                return this;
            }

            var dateTimeString = ParseDate(value);
            _post.TryAdd(fieldName, dateTimeString);

            return this;
        }

        private string ParseDate(string value)
        {
            if (_dateTimeString != null
                && DateTime.TryParseExact(
                value,
                _dateTimeString,
                null,
                System.Globalization.DateTimeStyles.None,
                out var exactTime))
            {
                return exactTime.ToString("s");
            }

            if (DateTime.TryParse(value, out var datetime))
            {
                return datetime.ToString("s");
            }

            return DateTime.MinValue.ToString("s");
        }
    }
}
