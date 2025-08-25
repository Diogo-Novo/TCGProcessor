using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TCGProcessor.Models
{
    public class JobInfo
    {
        public string JobId { get; set; }
        public string UserId { get; set; }
        public string Status { get; set; }
        public string Message { get; set; }
        public int Progress { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public ProcessingResult Result { get; set; }
        public string Error { get; set; }
    }
}