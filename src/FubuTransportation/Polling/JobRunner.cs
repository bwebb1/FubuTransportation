﻿using System;

namespace FubuTransportation.Polling
{
    public class JobRunner<T> where T : IJob
    {
        private readonly T _job;
        private readonly IPollingJobLogger _logger;

        public JobRunner(T job, IPollingJobLogger logger)
        {
            _job = job;
            _logger = logger;
        }

        public void Run(JobRequest<T> request)
        {
            _logger.Starting(_job);

            try
            {
                _job.Execute();
                _logger.Successful(_job);
            }
            catch (Exception e)
            {
                _logger.Failed(_job, e);
                // TODO -- apply retry rules!
            }
        }
    }
}