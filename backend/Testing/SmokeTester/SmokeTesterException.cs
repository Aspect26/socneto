using System;

namespace SmokeTester
{
    public class SmokeTesterException:Exception
    {
        private const string ErrorMessageTemplate = "Call to route \n{0}\nfailed. Info: \n{1}";

        public SmokeTesterException(string route, string message)
            :base( string.Format(ErrorMessageTemplate,route,message))
        {
            
        }
        public SmokeTesterException(string route, string message, Exception innerException)
            : base(string.Format(ErrorMessageTemplate, route, message),innerException)
        {

        }
    }
}