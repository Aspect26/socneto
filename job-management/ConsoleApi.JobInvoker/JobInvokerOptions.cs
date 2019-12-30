using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;

namespace ConsoleApi.JobInvoker
{
    public class JobInvokerOptions
    {
        [Required]
        public Uri JobSubmitUri { get; set; }
    }

}
