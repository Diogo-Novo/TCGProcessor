using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TCGProcessor.Interfaces;
using TCGProcessor.Models;

namespace TCGProcessor.Services
{
    public class JobTrackerService : IJobTracker
    {
        private readonly ConcurrentDictionary<string, JobInfo> _jobs = new();

        public void StartJob(string jobId)
        {
            _jobs[jobId] = new JobInfo
            {
                JobId = jobId,
                Status = "Started",
                StartTime = DateTime.UtcNow
            };
        }

        public void UpdateProgress(string jobId, int progress, string status)
        {
            if (_jobs.TryGetValue(jobId, out var job))
            {
                job.Progress = progress;
                job.Status = status;
            }
        }

        public void FailJob(string jobId, string error)
        {
            if (_jobs.TryGetValue(jobId, out var job))
            {
                job.Status = "Failed";
                job.Error = error;
                job.EndTime = DateTime.UtcNow;
            }
        }

        public void CompleteJob(string jobId, ProcessingResult result)
        {
            if (_jobs.TryGetValue(jobId, out var job))
            {
                job.Status = "Completed";
                job.Result = result;
                job.EndTime = DateTime.UtcNow;
            }
        }

        public JobInfo? GetJobStatus(string jobId)
        {
            _jobs.TryGetValue(jobId, out var job);
            return job;
        }
    }
}