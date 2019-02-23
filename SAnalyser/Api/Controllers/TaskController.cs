using System;
using Domain;
using Domain.Models;
using Microsoft.AspNetCore.Mvc;
using SAnalyser.Api.Models;

namespace SAnalyser.Api.Controllers
{
    [ApiController]
    public class TaskController : ControllerBase
    {
        private readonly TaskService _taskService;

        public TaskController(TaskService taskService)
        {
            _taskService = taskService;
        }

        [HttpPost]
        [Route("api/submit")]
        public ActionResult<TaskResponse> Submit([FromBody] TaskRequest taskRequest)
        {
            var taskInput = ParseTaskRequest(taskRequest);
            var result = _taskService.ProcessTaskAsync(taskInput).Result;
            var taskResponse = TaskResponse.FromResult(result);
            return Ok(taskResponse);
        }

        private TaskInput ParseTaskRequest(TaskRequest taskRequest)
        {
            if (string.IsNullOrEmpty(taskRequest.Topic))
                throw new ArgumentException("Invalid topic");

            var fromDate = ParseW3cDateString(taskRequest.FromDate);
            var toDate = ParseW3cDateString(taskRequest.ToDate);

            return new TaskInput()
            {
                FromDate = fromDate,
                ToDate = toDate,
                Topic = taskRequest.Topic
            };
        }

        private DateTime ParseW3cDateString(string date)
        {
            string formatString = "yyyy-MM-ddTHH:mm:ss.fffZ";

            System.Globalization.CultureInfo cInfo = new System.Globalization.CultureInfo("en-US", true);

            try
            {
                return DateTime.ParseExact(date, formatString, cInfo);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }
}
