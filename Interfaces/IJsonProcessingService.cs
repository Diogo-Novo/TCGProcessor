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
        Task<ValidationResult> ValidateCards(List<ManaBoxCardCsvRecord> cards);
        Task<PsPricingSheet> GenerateManaBoxPricingSheetItems(List<EnrichedMTGCard> enrichedCards, JsonProcessingRequest request);
    }
}