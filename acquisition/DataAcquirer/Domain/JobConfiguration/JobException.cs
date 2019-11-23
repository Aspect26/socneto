using System;

namespace Domain.JobConfiguration
{
    public class JobException : Exception
    {
        public JobException(string message):base(message)
        {

        }
        public JobException(string message, Exception inner):base(message,inner)
        {

        }
    }
}
