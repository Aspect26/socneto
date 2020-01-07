using System;

namespace Tests.Integration
{
    public class AcquirerAssertException : Exception
    {
        public AcquirerAssertException(string message) : base(message)
        {
        }

        public AcquirerAssertException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }


}
