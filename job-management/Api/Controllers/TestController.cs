using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Api.Models;
using Domain.Abstract;
using Domain.ComponentManagement;
using Domain.Models;
using Domain.SubmittedJobConfiguration;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Api.Controllers
{
    [ApiController]
    public class TestController : ControllerBase
    {
        private readonly IMessageBrokerProducer _producer;
        private readonly IComponentRegistry _componentRegistry;

        public TestController(
            IMessageBrokerProducer producer,
            IComponentRegistry componentRegistry)
        {
            _producer = producer;
            _componentRegistry = componentRegistry;
        }

        [HttpGet]
        [Route("/api/test/say-hello")]
        public async Task<ActionResult> SayHelloAsync()
        {
            return Ok(new { message = "hello" });
        }

        [HttpGet]
        [Route("/api/test/produce")]
        public async Task<ActionResult> ProduceAsync()
        {
            await _producer.ProduceAsync("foobar", new Domain.MessageBrokerMessage("key", "{}"));
            return Ok(new { message = "send" });
        }

        [HttpGet]
        [Route("/api/test/read-storage")]
        public async Task<ActionResult> ReadStorageAsync()
        {
            await _componentRegistry.AddOrUpdateAsync(
                new ComponentModel(
                    "cid",
                    "DATA_ANALYSER",
                    "hcaneel",
                    "upda",
                    new JObject()
                    ));
            var component = await _componentRegistry.GetComponentByIdAsync("cid");
            return Ok(new { comp = component  });
        }

    }
}
