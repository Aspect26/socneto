﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;

namespace Api
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
#if DEBUG
            args = new[]
            {
                //"--no_kafka",
                "--use_file_storage"
            };
#endif

            if (args.Contains("--sleep_on_startup"))
            {
                await Task.Delay(TimeSpan.FromSeconds(90));
            }

            var builder = JobManagementServiceBuilder.GetBuilder(args);
            var app = builder.BuildJobManagementService();

            await app.RunAsync();
        }
    }
}
