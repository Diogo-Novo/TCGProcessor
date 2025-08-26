using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Scryfall.API.Models;
using TCGProcessor.Data;
using TCGProcessor.Models.Cache;

namespace TCGProcessor.Repositories
{
    public class CacheRepository
    {
        private readonly OSMGXProcessorDbContext _context;

        public CacheRepository(OSMGXProcessorDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Try to get a cached Scryfall card by ID.
        /// Returns (true, Card) if found & valid,
        /// (false, null) if not found or expired.
        /// </summary>
        public async Task<(bool found, Card? card)> GetCachedCardAsync(Guid cardId)
        {
            var cachedCard = await _context
                .ScryfallCache.AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == cardId);

            if (cachedCard == null || cachedCard.IsExpired())
                return (false, null);

            if (cachedCard.IsFound && cachedCard != null)
            {
                var card = cachedCard.CardData;
                return (true, card);
            }

            return (false, null);
        }

        /// <summary>
        /// Save or update a cached card.
        /// </summary>
        public async Task SaveCachedCardAsync(
            Guid cardId,
            Card? card,
            bool isFound,
            string? error = null
        )
        {
            var cachedCard = new CachedScryfallCard
            {
                Id = cardId,
                CardData = card == null ? null : card,
                CachedAt = DateTime.UtcNow,
                IsFound = isFound,
                Error = error
            };

            _context.ScryfallCache.Update(cachedCard);
            await _context.SaveChangesAsync();
        }
    }
}
