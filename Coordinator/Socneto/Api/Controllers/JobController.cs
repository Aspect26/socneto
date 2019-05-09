using System;
using System.Collections.Generic;
using System.Linq;
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

            var status = new
            {
                TimeStamp = DateTime.Now.ToString("s")
            };

            return Ok(status);
        }

        [HttpPost]
        [Route("api/submit")]
        public async Task<ActionResult<JobResponse>> Submit([FromBody] JobRequest taskRequest)
        {
            var taskInput = JobRequest.ToTaskInput(taskRequest);
            var taskResult = await _jobService.SubmitJob(taskInput);

            var taskResponse = new JobResponse() { JobId = taskResult.JobId };
            return Ok(taskResponse);
        }

        [HttpGet]
        [Route("api/job-status/{jobId:guid}")]
        public async Task<ActionResult<JobStatusResponse>> GetJobStatus([FromRoute]Guid jobId)
        {
            var jobStatusResponse = GetRandomJobStatusResponse(jobId);

            return Ok(jobStatusResponse);
        }



        [HttpGet]
        [Route("api/user-job-statuses/{userId}")]
        public async Task<ActionResult<List<JobStatusResponse>>> GetJobStatuses([FromRoute]int userId)
        {
            var random = new Random(userId);

            return Enumerable.Range(0, random.Next(5, 15))
                .Select(r =>
                {
                    var arr = new byte[16];
                    random.NextBytes(arr);
                    return new Guid(arr);
                })
                .Select(GetRandomJobStatusResponse)
                .ToList();
        }

        [HttpGet]
        [Route("api/job-result/{jobId:guid}")]
        public async Task<ActionResult<JobResultResponse>> GetJobResult([FromRoute]Guid jobId)
        {
            var hc = Math.Abs(jobId.GetHashCode());
            var topics = new[] { "Guns", "Cars", "Friends", "Cartoon", "Sunshine" };

            var posts = Enumerable.Range(0, hc % 100)
                .Select(r =>
                {
                    var rand = new Random(r + hc).Next(int.MaxValue);
                    return new PostDto
                    {
                        Keywords = new List<string>() { topics[rand % topics.Length] },
                        Sentiment = (double)(rand % 2),
                        Text = RandomString(64 + rand % 64)
                    };
                })
                .ToList();


            var jobResultResponse = new JobResultResponse
            {
                InputQuery = topics[hc % topics.Length],
                Posts = posts
            };

            return Ok(jobResultResponse);
        }

        private static JobStatusResponse GetRandomJobStatusResponse(Guid jobId)
        {

            var hc = Math.Abs(jobId.GetHashCode());

            var year = 2010 + (hc % 10);
            var month = hc % 12 + 1;
            var day = hc % 28 + 1;
            var hour = hc % 24 + 1;
            var minute = hc % 60 + 1;
            var second = hc % 60 + 1;
            var startedAt = new DateTime(year, month, day, hour, minute, second);

            DateTime? finishedAt = null;
            if (hc % 5 == 0)
            {
                finishedAt = startedAt + new TimeSpan(hc % 100, hc % 24, hc % 60);
            }

            var jobStatusResponse = new JobStatusResponse()
            {
                JobId = jobId,
                StartedAt = startedAt,
                FinishedAt = finishedAt,
                HasFinished = finishedAt.HasValue,
                UserId = hc
            };
            return jobStatusResponse;

        }

        public static string RandomString(int length)
        {
            var random = new Random(DateTime.Now.GetHashCode());
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }




}
