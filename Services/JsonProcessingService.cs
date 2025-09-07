using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TCGProcessor.Interfaces;
using TCGProcessor.Models;
using TCGProcessor.Repositories;

namespace TCGProcessor.Services
{
    public class JsonProcessingService : IJsonProcessingService
    {
        private readonly IScryfallService _scryfallService;
        private readonly IWebHostEnvironment _environment;
        private readonly ILogger<JsonProcessingService> _logger;
        private readonly PricingSheetRepository _pricingSheetRepository;

        public JsonProcessingService(
            IScryfallService scryfallService,
            IWebHostEnvironment environment,
            ILogger<JsonProcessingService> logger,
            PricingSheetRepository pricingSheetRepository
        )
        {
            _scryfallService = scryfallService;
            _environment = environment;
            _logger = logger;
            _pricingSheetRepository = pricingSheetRepository;
        }

        public async Task<List<EnrichedMTGCard>> EnrichWithScryfallData(
            List<ManaBoxCardCsvRecord> cards,
            Func<ProgressInfo, Task> progressCallback
        )
        {
            var enrichedCards = new List<EnrichedMTGCard>();
            var processed = 0;

            _logger.LogInformation("Starting enrichment of {CardCount} cards", cards.Count);

            foreach (var card in cards)
            {
                try
                {
                    var scryfallData = await _scryfallService.GetCardByIdAsync(card.ScryfallId);

                    enrichedCards.Add(
                        new EnrichedMTGCard { OriginalCard = card, ScryfallData = scryfallData }
                    );

                    processed++;
                    await progressCallback(
                        new ProgressInfo { Current = processed, Total = cards.Count }
                    );
                    _logger.LogInformation("Progress callback executed: {Current}/{Total}", processed, cards.Count);

                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Error processing card {CardName}", card.Name);

                    enrichedCards.Add(
                        new EnrichedMTGCard
                        {
                            OriginalCard = card,
                            ScryfallData = null,
                            Error = ex.Message
                        }
                    );

                    processed++;
                    await progressCallback(
                        new ProgressInfo { Current = processed, Total = cards.Count }
                    );
                }
            }

            _logger.LogInformation(
                "Completed enrichment. {ProcessedCount} cards processed",
                processed
            );
            return enrichedCards;
        }

        public async Task<PsPricingSheet> GenerateManaBoxPricingSheetItems(
            List<EnrichedMTGCard> enrichedCards,
            JsonProcessingRequest request
        )
        {
            int totalCards = enrichedCards.Count;
            int processedCards = 0;
            int errorCount = 0;
            PsPricingSheet pricingSheet = _pricingSheetRepository.GetPricingSheetById(
                request.PricingSheetId
            );
            List<PsPricingSheetItem> pricingItems = new();

            foreach (var card in enrichedCards)
            {
                #region Skip Logic
                if (card.OriginalCard.Rarity == "common" && !request.Config.IncludeCommon)
                {
                    processedCards++;
                    continue; // Skip common cards if not included
                }
                if (card.OriginalCard.Rarity == "uncommon" && !request.Config.IncludeUncommon)
                {
                    processedCards++;
                    continue; // Skip uncommon cards if not included
                }
                if (card.OriginalCard.Rarity == "rare" && !request.Config.IncludeRare)
                {
                    processedCards++;
                    continue; // Skip rare cards if not included
                }
                if (card.OriginalCard.Rarity == "mythic" && !request.Config.IncludeMythic)
                {
                    processedCards++;
                    continue; // Skip mythic cards if not included
                }
                #endregion

                //Calculate pricing based on Scryfall data and config
                decimal finalCalculatedPriceGbp = 0;

                try
                {
                    // Placeholder for actual pricing logic
                    decimal calculatedPriceGbpFromUsd = 0;
                    decimal calculatedPriceGbpFromEur = 0;

                    switch (card.OriginalCard.Finish)
                    {
                        case CardFinish.Etched:
                            finalCalculatedPriceGbp =
                                (card.ScryfallData?.Prices.UsdEtched ?? 0) * request.UsdToGbpRate;
                            break;
                        case CardFinish.Foil:
                            calculatedPriceGbpFromUsd =
                                (card.ScryfallData?.Prices.UsdFoil ?? 0) * request.UsdToGbpRate;
                            calculatedPriceGbpFromEur =
                                (card.ScryfallData?.Prices.EurFoil ?? 0) * request.EuroToGbpRate;
                            finalCalculatedPriceGbp = CalculateAveragePrice(
                                calculatedPriceGbpFromUsd,
                                calculatedPriceGbpFromEur
                            );
                            break;

                        case CardFinish.Normal:
                        default:
                            calculatedPriceGbpFromUsd =
                                (card.ScryfallData?.Prices.Usd ?? 0) * request.UsdToGbpRate;
                            calculatedPriceGbpFromEur =
                                (card.ScryfallData?.Prices.Eur ?? 0) * request.EuroToGbpRate;
                            finalCalculatedPriceGbp = CalculateAveragePrice(
                                calculatedPriceGbpFromUsd,
                                calculatedPriceGbpFromEur
                            );
                            break;
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(
                        ex,
                        "Error calculating pricing for card {CardName}",
                        card.OriginalCard.Name
                    );
                    errorCount++;
                    processedCards++;
                    continue;
                }

                if (finalCalculatedPriceGbp < request.Config.MinPrice)
                {
                    // Skip cards with no price
                    processedCards++;
                    continue;
                }

                for (int i = 0; i < card.OriginalCard.Quantity; i++)
                {
                    pricingItems.Add(
                        new PsPricingSheetItem
                        {
                            PsiCreatedBy = pricingSheet.PsCreatedBy,
                            PsiSaleValue = finalCalculatedPriceGbp,
                            PsiItemName =
                                $"{card.OriginalCard.Name.ToUpper()} ({card.OriginalCard.CollectorNumber} - {card.OriginalCard.SetCode.ToUpper()}) {card.OriginalCard.Finish.ToString().ToUpper()}",
                            PsiCashValue = finalCalculatedPriceGbp * 0.6m,
                            PsiTradeValue = finalCalculatedPriceGbp * 0.6m,
                            PsiPricingSheet = request.PricingSheetId,
                        }
                    );
                }

                processedCards++; // Increment processed cards after successful processing
            }

            // Update pricing sheet with calculated values
            pricingSheet.PsItemCount = pricingItems.Count;
            pricingSheet.PsTotalSellValue = pricingItems.Sum(i => i.PsiSaleValue);
            pricingSheet.PsTotalCashValue = pricingItems.Sum(i => i.PsiCashValue);
            pricingSheet.PsTotalTradeValue = pricingItems.Sum(i => i.PsiTradeValue);
            pricingSheet.PsLastUpdated = DateTime.UtcNow;
            pricingSheet.PsPricingSheetItems = pricingItems;

            // Update the pricing sheet in the database
            await _pricingSheetRepository.UpdatePricingSheetAsync(pricingSheet);

            // Log completion statistics
            _logger.LogInformation(
                "Pricing sheet generation completed. Processed: {ProcessedCards}, Errors: {ErrorCount}, Total Items: {TotalItems}",
                processedCards,
                errorCount,
                pricingItems.Count
            );

            return pricingSheet;
        }

        // Helper method for calculating average price
        private decimal CalculateAveragePrice(decimal usdPrice, decimal eurPrice)
        {
            if (usdPrice > 0 && eurPrice > 0)
            {
                return Math.Max(usdPrice, eurPrice);
            }
            else if (eurPrice > 0)
            {
                return eurPrice;
            }
            else if (usdPrice > 0)
            {
                return usdPrice;
            }
            return 0;
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
