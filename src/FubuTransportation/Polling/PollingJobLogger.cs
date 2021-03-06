﻿using System;
using FubuCore.Logging;
using FubuMVC.Core.Behaviors;
using FubuCore;

namespace FubuTransportation.Polling
{
    public class PollingJobLogger : IPollingJobLogger
    {
        private readonly ILogger _logger;

        public PollingJobLogger(ILogger logger)
        {
            _logger = logger;
        }

        public void Stopping(Type jobType)
        {
            _logger.InfoMessage(() => new PollingJobStopped{JobType = jobType});
        }

        public void Starting(IJob job)
        {
            _logger.InfoMessage(() => new PollingJobStarted() { Description = job.ToString() });
        }

        public void Successful(IJob job)
        {
            _logger.InfoMessage(() => new PollingJobSuccess{Description = job.ToString()});
        }

        public void Failed(IJob job, Exception ex)
        {
            _logger.Error("Job {0}".ToFormat(job), ex);
            _logger.InfoMessage(() => new PollingJobFailed { Description = job.ToString(), Exception = ex});

        }

        public void FailedToSchedule(Type jobType, Exception exception)
        {
            _logger.Error("Job {0} could not be scheduled to run".ToFormat(jobType.FullName), exception);
        }
    }
}