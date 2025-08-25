using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace TCGProcessor.Models
{
    public class ManaBoxProcessingRequest
    {
        [Required]
        public List<ManaBoxCardCsvRecord> Cards { get; set; } = new();

        [Required]
        public ManaBoxImportConfig Config { get; set; } = new();

        public decimal EuroToGbpRate { get; set; } = 0.85m;
        public decimal UsdToGbpRate { get; set; } = 0.79m;
        
        public int PricingSheetId { get; set; }
    }
}