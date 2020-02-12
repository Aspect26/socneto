using System;
using System.Runtime.Serialization;

namespace Domain
{
    public class JobManagementException : Exception
    {
        public JobManagementException()
        {
        }

        public JobManagementException(string message) : base(message)
        {
        }

        public JobManagementException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected JobManagementException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}