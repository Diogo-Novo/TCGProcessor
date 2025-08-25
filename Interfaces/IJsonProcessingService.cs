using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TCGProcessor.Models;

namespace TCGProcessor.Interfaces
{
    public interface IJsonProcessingService
    {
        Task<List<EnrichedMTGCard>> EnrichWithScryfallData(List<ManaBoxCardCsvRecord> cards, Func<ProgressInfo, Task> progressCallback);
        Task<PsPricingSheet> GeneratePricingSheet(List<EnrichedMTGCard> cards, ManaBoxImportConfig config);
        Task<string> SavePricingSheet(PsPricingSheet sheet, string jobId, string format);
        Task<ValidationResult> ValidateCards(List<ManaBoxCardCsvRecord> cards);
    }
}