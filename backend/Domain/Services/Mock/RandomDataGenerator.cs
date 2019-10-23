﻿using System;
using System.Linq;
using Socneto.Domain.Models;

namespace Socneto.Domain.Services.Mock
{
    public class RandomDataGenerator
    {
        public static Job GetRandomJobStatusResponse(Guid jobId)
        {
            var hc = Math.Abs(jobId.GetHashCode());

            var startedAt = GetRandomDate(hc);

            DateTime? pausedAt = null;
            if (hc % 5 == 0)
            {
                pausedAt = startedAt + new TimeSpan(hc % 100, hc % 24, hc % 60);
            }

            var jobStatusResponse = new Job()
            {
                JobId = jobId,
                StartedAt = startedAt,
                PausedAt = pausedAt,
                IsRunning = !pausedAt.HasValue,
                JobName = RandomString(10),
                Username = "admin"

            };
            return jobStatusResponse;
        }

        public static DateTime GetRandomDate(int seed)
        {
            var year = 2010 + (seed % 10);
            var month = (seed % 12) + 1;
            var day = (seed % 28) + 1;
            var hour = seed % 24;
            var minute = ((seed - 53) % 59) + 1;
            var second = (seed % 59) + 1;
            //Console.WriteLine($"{year} {month} {day} {hour} {minute} {second}");
            return new DateTime(year, month, day, hour, minute, second);
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