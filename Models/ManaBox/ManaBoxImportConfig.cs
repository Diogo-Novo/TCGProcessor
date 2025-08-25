using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TCGProcessor.Models
{
    public class ManaBoxImportConfig
    {
        public decimal MinPrice { get; set; }
        public bool IncludeCommon { get; set; }
        public bool IncludeUncommon { get; set; }
        public bool IncludeRare { get; set; }
        public bool IncludeMythic { get; set; }
    }
}