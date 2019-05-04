using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Socneto.Coordinator.Api.Models;
using Socneto.Coordinator.Domain;
using Socneto.Coordinator.Domain.Models;

namespace Socneto.Coordinator.Api.Controllers
{
    [ApiController]
    public class JobController : ControllerBase
    {
        private readonly IJobService _jobService;

        public JobController(IJobService jobService)
        {
            _jobService = jobService;
        }

        [HttpGet]
        [Route("api/heart-beat")]
        public ActionResult<string> HeartBeat()
        {
            return Ok($"Hello World! TC checked. time {DateTime.Now:O}");
        }
        
        [HttpPost]
        [Route("api/submit")]
        public async Task< ActionResult<JobResponse>> Submit([FromBody] JobRequest taskRequest)
        {
            var taskInput = JobRequest.ToTaskInput(taskRequest);

            
            var taskResult = await _jobService.SubmitJob(taskInput);

            var taskResponse = new JobResponse(){JobId =taskResult.JobId};
            return Ok( taskResponse );
        }

        //private JobInput ParseTaskRequest(TaskRequest taskRequest)
        //{
        //    if (string.IsNullOrEmpty(taskRequest.Topic))
        //        throw new ArgumentException("Invalid topic");

        //    var fromDate = ParseW3cDateString(taskRequest.FromDate);
        //    var toDate = ParseW3cDateString(taskRequest.ToDate);

        //    return new JobInput()
        //    {
        //        FromDate = fromDate,
        //        ToDate = toDate,
        //        Topic = taskRequest.Topic
        //    };
        //}

        //private DateTime ParseW3cDateString(string date)
        //{
        //    string formatString = "yyyy-MM-ddTHH:mm:ss.fffZ";

        //    System.Globalization.CultureInfo cInfo = new System.Globalization.CultureInfo("en-US", true);

        //    try
        //    {
        //        return DateTime.ParseExact(date, formatString, cInfo);
        //    }
        //    catch (Exception e)
        //    {
        //        Console.WriteLine(e);
        //        throw;
        //    }
        //}
    }
}
