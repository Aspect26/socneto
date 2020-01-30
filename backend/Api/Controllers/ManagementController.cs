using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Socneto.Api.Models;
using Socneto.Domain;
using Socneto.Domain.Models;
using Socneto.Domain.Services;
using Socneto.Infrastructure.Kafka;

namespace Socneto.Api.Controllers
{
    [ApiController]
    public class ManagementController : ControllerBase
    {

        private readonly IJobManagementService _jobManagementService;
        private readonly IStorageService _storageService;
        
        private readonly ILogger<KafkaProducer> _kafkaLogger;
        private readonly ILogger<ManagementController> _logger;

        public ManagementController(IJobManagementService jobManagementService, IStorageService storageService, 
            ILogger<KafkaProducer> kafkaLogger, ILogger<ManagementController> logger)
        {
            _jobManagementService = jobManagementService;
            _storageService = storageService;
            _kafkaLogger = kafkaLogger;
            _logger = logger;
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
        public async Task<ActionResult> Produce([FromBody] ProduceRequest request)
        {
            _logger.LogInformation("Producing");
            try
            {
                var kafkaOptions = Options.Create(new KafkaOptions {ServerAddress = request.ServerAddress});
                var taskOptions =
                    Options.Create(new TaskOptions {ConsumeTaskTopic = "n/a", ProduceTopic = request.KafkaTopic});
                var kafka = new KafkaProducer(kafkaOptions, taskOptions, _kafkaLogger);

                var message = new Message()
                {
                    Key = "key",
                    Value = request.Message
                };
                await kafka.ProduceAsync(message);
            }
            catch (Exception e)
            {
                _logger.LogError("error :{message}",e.Message);
            }

            return Ok();
        }

        [HttpGet]
        [Route("api/platform_status")]
        public async Task<ActionResult<SocnetoComponentsStatus>> PlatformStatus()
        {
            var jmsStatus = await _jobManagementService.ComponentRunning()
                ? SocnetoComponentStatus.Running
                : SocnetoComponentStatus.Stopped;

            var storageStatus = await _storageService.ComponentRunning()
                ? SocnetoComponentStatus.Running
                : SocnetoComponentStatus.Stopped;

            return new SocnetoComponentsStatus
            {
                JmsStatus = jmsStatus,
                StorageStatus = storageStatus
            };
        }
    }
}