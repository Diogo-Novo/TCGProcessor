using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TCGProcessor.Interfaces;
using TCGProcessor.Models;

namespace TCGProcessor.Services
{
    public class JsonProcessingService : IJsonProcessingService
    {
        private readonly IScryfallService _scryfallService;
        private readonly IWebHostEnvironment _environment;
        private readonly ILogger<JsonProcessingService> _logger;

        public JsonProcessingService(
            IScryfallService scryfallService, 
            IWebHostEnvironment environment,
            ILogger<JsonProcessingService> logger)
        {
            _scryfallService = scryfallService;
            _environment = environment;
            _logger = logger;
        }

        public async Task<List<EnrichedMTGCard>> EnrichWithScryfallData(List<ManaBoxCardCsvRecord> cards, Func<ProgressInfo, Task> progressCallback)
        {
            var enrichedCards = new List<EnrichedMTGCard>();
            var processed = 0;

            _logger.LogInformation("Starting enrichment of {CardCount} cards", cards.Count);

            foreach (var card in cards)
            {
                try
                {
                    var scryfallData = await _scryfallService.GetCardById(card.ScryfallId);

                    enrichedCards.Add(new EnrichedMTGCard
                    {
                        OriginalCard = card,
                        ScryfallData = scryfallData
                    });

                    processed++;
                    await progressCallback(new ProgressInfo { Current = processed, Total = cards.Count });
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Error processing card {CardName}", card.Name);

                    enrichedCards.Add(new EnrichedMTGCard
                    {
                        OriginalCard = card,
                        ScryfallData = null,
                        Error = ex.Message
                    });

                    processed++;
                    await progressCallback(new ProgressInfo { Current = processed, Total = cards.Count });
                }
            }

            _logger.LogInformation("Completed enrichment. {ProcessedCount} cards processed", processed);
            return enrichedCards;
        }

        public async Task<PsPricingSheet> GeneratePricingSheet(List<EnrichedMTGCard> cards, ManaBoxImportConfig config)
        {
            throw new NotImplementedException();
        }

        public async Task<string> SavePricingSheet(PsPricingSheet sheet, string jobId, string format)
        {
            throw new NotImplementedException();
        }

        public async Task<ValidationResult> ValidateCards(List<ManaBoxCardCsvRecord> cards)
        {
            var result = new ValidationResult { IsValid = true };
            var validCards = 0;
            var invalidCards = 0;

            foreach (var (card, index) in cards.Select((c, i) => (c, i)))
            {
                if (string.IsNullOrWhiteSpace(card.Name))
                {
                    result.Errors.Add($"Card at index {index}: Name is required");
                    invalidCards++;
                    continue;
                }

                if (string.IsNullOrWhiteSpace(card.ScryfallId))
                {
                    result.Warnings.Add($"Card '{card.Name}': No Scryfall ID provided");
                }

                if (card.Quantity <= 0)
                {
                    result.Errors.Add($"Card '{card.Name}': Quantity must be greater than 0");
                    invalidCards++;
                    continue;
                }

                validCards++;
            }

            result.ValidCards = validCards;
            result.InvalidCards = invalidCards;
            result.IsValid = result.Errors.Count == 0;

            return result;
        }
    }
}