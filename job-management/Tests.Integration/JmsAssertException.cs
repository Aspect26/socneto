using System;

namespace Tests.Integration
{
    public class JmsAssertException : Exception
    {
        public JmsAssertException(string message) : base(message)
        {
        }

        public JmsAssertException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }


}
