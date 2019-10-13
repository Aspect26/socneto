using System.Collections.Generic;

namespace Domain.Model
{
    public class DataAcquirerAttributes
    {
        private Dictionary<string,string> _attributes = new Dictionary<string, string>();

        public string this[string index]
        {
            get { return _attributes[index]; }
        }

        public DataAcquirerAttributes( IDictionary<string,string> attributes)
        {
            foreach (var (key,value) in attributes)
            {
                _attributes[key] = value;
            }
        }
    }
}