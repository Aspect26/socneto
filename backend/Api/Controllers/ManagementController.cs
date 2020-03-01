using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Socneto.Api.Models;
using Socneto.Api.Models.Requests;
using Socneto.Api.Models.Responses;
using Socneto.Domain;
using Socneto.Domain.EventTracking;
using Socneto.Domain.Models;
using Socneto.Domain.Services;
using Socneto.Infrastructure.Kafka;

namespace Socneto.Api.Controllers
{
    [ApiController]
    public class ManagementController : SocnetoController
    {

        private readonly IJobManagementService _jobManagementService;
        private readonly IStorageService _storageService;
        
        private readonly ILogger<KafkaResultProducer> _kafkaLogger;
        private readonly IEventTracker<ManagementController> _eventTracker;

        public ManagementController(
            IJobManagementService jobManagementService, 
            IStorageService storageService, 
            ILogger<KafkaResultProducer> kafkaLogger,
            IEventTracker<ManagementController> logger)
        {
            _jobManagementService = jobManagementService;
            _storageService = storageService;
            _kafkaLogger = kafkaLogger;
            _eventTracker = logger;
        }

        [HttpGet]
        [Route("api/heart-beat")]
        public ActionResult<string> HeartBeat()
        {
            _eventTracker.TrackInfo("HeartBeat", "Heart beat requested");
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
            _eventTracker.TrackInfo("JmsDebugProducing","produced message");
            try
            {
                var kafkaOptions = Options.Create(new KafkaOptions {ServerAddress = request.ServerAddress});
                var taskOptions =
                    Options.Create(new TaskOptions {ConsumeTaskTopic = "n/a", ProduceTopic = request.KafkaTopic});
                var kafka = new KafkaResultProducer(kafkaOptions, taskOptions, _kafkaLogger);

                var message = new Message()
                {
                    Key = "key",
                    Value = request.Message
                };
                await kafka.ProduceAsync(message);
            }
            catch (Exception e)
            {
                _eventTracker.TrackError(
                    "JmsDebugProducing",
                    $"Error while producing: {e.Message}", 
                    new { exception = e });
            }

            return Ok();
        }

        [EnableCors("_myAllowSpecificOrigins")]
        [HttpGet]
        [Route("api/platform_status")]
        public async Task<ActionResult<SocnetoComponentsStatus>> PlatformStatus()
        {
            var jmsStatus = await _jobManagementService.IsComponentRunning()
                ? SocnetoComponentStatus.Running
                : SocnetoComponentStatus.Stopped;

            var storageStatus = await _storageService.IsComponentRunning()
                ? SocnetoComponentStatus.Running
                : SocnetoComponentStatus.Stopped;
            
            var response = new SocnetoComponentsStatus
            {
                JmsStatus = jmsStatus,
                StorageStatus = storageStatus
            };
            
            _eventTracker.TrackInfo("PlatformStatus", "Platform status requested", response);

            return Ok(response);
        }
    }
}