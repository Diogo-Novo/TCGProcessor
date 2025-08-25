using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TCGProcessor.Models
{
    public class ProcessingStats
    {
        public int TotalCards { get; set; }
        public int ProcessedCards { get; set; }
        public int FoundCards { get; set; }
        public int NotFoundCards { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime? EndTime { get; set; }
    }
}