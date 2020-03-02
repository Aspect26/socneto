using System;

namespace Tests.Integration
{
    public class BackendAssertException : Exception
    {
        public BackendAssertException(string message) : base(message)
        {
        }

        public BackendAssertException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }


    public class BackendTestException : Exception
    {
        public BackendTestException(string message) : base(message)
        {
        }

        public BackendTestException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }


}
