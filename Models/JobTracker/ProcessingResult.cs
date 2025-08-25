using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TCGProcessor.Models
{
    public class ProcessingResult
    {
        public string JobId { get; set; }
        public bool Success { get; set; }
        public string Message { get; set; }
        public string DownloadUrl { get; set; }
        public ProcessingStats Stats { get; set; }
    }
}