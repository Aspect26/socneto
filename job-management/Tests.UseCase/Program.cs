using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Domain.ComponentManagement;
using Domain.Models;
using Infrastructure;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;

namespace Tests.UseCase
{
    class Program
    {
        static void Main(string[] args)
        {
            /*
             *  "AnalyserComponentTypeName": "DATA_ANALYSER",
      "DataAcquirerComponentTypeName": "DATA_ACQUIRER"
    },
    "ComponentStorageOptions": {
      "BaseUri": "http://10.7.31.161:8888",
      "AddOrUpdateComponentRoute": "/components",
      "GetComponentRoute": "/components/componentId"
             */

            var logger = new Logger<ComponentStorageProxy>(new NullLoggerFactory());

            var o1 = Options.Create(new ComponentStorageOptions
            {
                BaseUri = "http://10.7.31.161:8888",
                AddOrUpdateComponentRoute = "components",
                GetComponentRoute = "/components/componentId"
            });
            var o2 = Options.Create(new ComponentIdentifiers
            {
                AnalyserComponentTypeName = "DATA_ANALYSER",
                DataAcquirerComponentTypeName = "DATA_ACQUIRER"
            });
            var proxy = new ComponentStorageProxy(
                new HttpClient(),
                o1,
                o2,
                logger
                );

            var cmp = new ComponentRegistrationModel(
                "jk_test",
                "test.update",
                "test.input",
                o2.Value.AnalyserComponentTypeName,
                new Dictionary<string, string>()
                );
            proxy.AddOrUpdateAsync(cmp).GetAwaiter().GetResult();

            var mycmp = proxy.GetComponentById(cmp.ComponentId).GetAwaiter().GetResult();


        }
    }
}
