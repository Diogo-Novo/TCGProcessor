using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TCGProcessor.Models;

namespace TCGProcessor.Interfaces
{
    public interface IBackgroundTaskQueue
    {
        void QueueJob(JsonProcessingRequest request);
        bool TryDequeue(out JsonProcessingRequest request);
    }
}
