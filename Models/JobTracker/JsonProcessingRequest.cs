using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TCGProcessor.Models
{
    public class JsonProcessingRequest
    {
        public string JobId { get; set; } = string.Empty;
        public List<ManaBoxCardCsvRecord> Cards { get; set; } = new();
        public ManaBoxImportConfig Config { get; set; } = new();
        public decimal EuroToGbpRate { get; set; }
        public decimal UsdToGbpRate { get; set; }
        public int PricingSheetId { get; set; }
    }
}