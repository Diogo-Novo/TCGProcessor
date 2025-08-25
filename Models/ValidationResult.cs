using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TCGProcessor.Models
{
    public class ValidationResult
    {
        public bool IsValid { get; set; }
        public List<string> Errors { get; set; } = new();
        public List<string> Warnings { get; set; } = new();
        public int ValidCards { get; set; }
        public int InvalidCards { get; set; }
    }
}