using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Domain.Exceptions
{
    public class DataAcquirerException : Exception
    {
        public DataAcquirerException()
        {
        }

        public DataAcquirerException(string message) : base(message)
        {
        }

        public DataAcquirerException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected DataAcquirerException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
