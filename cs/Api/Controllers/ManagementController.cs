﻿using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Socneto.Api.Models;
using Socneto.Domain;
using Socneto.Domain.Models;
using Socneto.Infrastructure.Kafka;

namespace Socneto.Api.Controllers
{
    [ApiController]
    public class ManagementController : ControllerBase
    {
        private readonly ILogger<KafkaProducer> _kafkaLogger;


        public ManagementController(ILogger<KafkaProducer> kafkaLogger)
        {
            this._kafkaLogger = kafkaLogger;
        }

        [HttpGet]
        [Route("api/heart-beat")]
        public ActionResult<string> HeartBeat()
        {

            var status = new
            {
                TimeStamp = DateTime.Now.ToString("s")
            };

            return Ok(status);
        }

        [HttpPost]
        [Route("api/produce")]
        public async Task< ActionResult> Produce([FromBody]ProduceRequest request)
        {
            var kafkaOptions = Options.Create(new KafkaOptions { ServerAddress = request.ServerAddress });
            var taskOptions =
                Options.Create(new TaskOptions {ConsumeTaskTopic = "n/a", ProduceTopic = request.KafkaTopic});
            var kafka = new KafkaProducer(kafkaOptions, taskOptions, _kafkaLogger);

            var message = new Message()
            {
                Key = "key",
                Value = request.Message
            };
            await kafka.ProduceAsync(message);

            return Ok();
        }
    }
}