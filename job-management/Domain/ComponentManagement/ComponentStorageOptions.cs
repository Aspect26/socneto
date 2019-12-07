using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Domain.Models;
using Microsoft.Extensions.Options;

namespace Domain.ComponentManagement
{
    public class ComponentStorageOptions
    {
        public string BaseUri { get; set; }
        public string AddOrUpdateComponentRoute { get; set; }
        public string GetComponentRoute { get; set; }

        public string ComponentJobConfigRoute { get; set; }

    }
    
}
