using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TCGProcessor.Models;

namespace TCGProcessor.Interfaces
{
    public interface IJobTracker
    {
        void StartJob(string jobId);
        void UpdateProgress(string jobId, int progress, string status);
        void CompleteJob(string jobId, ProcessingResult result);
        void FailJob(string jobId, string error);

        JobInfo? GetJobStatus(string jobId);
    }
}