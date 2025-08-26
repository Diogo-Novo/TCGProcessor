using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TCGProcessor.Interfaces;
using TCGProcessor.Models;

namespace TCGProcessor.Services
{
    public class BackgroundTaskQueue : IBackgroundTaskQueue
    {
        private readonly ConcurrentQueue<JsonProcessingRequest> _jobs = new();

        public void QueueJob(JsonProcessingRequest request)
        {
            _jobs.Enqueue(request);
        }

        public bool TryDequeue(out JsonProcessingRequest request)
        {
            return _jobs.TryDequeue(out request);
        }
    }
}