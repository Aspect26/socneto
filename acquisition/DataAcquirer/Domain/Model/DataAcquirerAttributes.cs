using System;
using System.Collections.Generic;

namespace Domain.Model
{
    public class DataAcquirerAttributes
    {
        private readonly Dictionary<string,string> _attributes 
            = new Dictionary<string, string>();

        public DataAcquirerAttributes( IDictionary<string,string> attributes)
        {
            foreach (var (key,value) in attributes)
            {
                _attributes[key] = value;
            }
        }

        public string GetValue(string key, string defaultValue = default)
        {
            return _attributes.GetValueOrDefault(key,defaultValue);
        }
    }
}
