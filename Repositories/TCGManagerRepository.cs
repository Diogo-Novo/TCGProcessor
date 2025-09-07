using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TCGProcessor.Data;
using TCGProcessor.Models;

namespace TCGProcessor.Repositories
{
    public class TCGManagerRepository
    {
        private readonly OSMGXTCGSystemDbContext _context;

        public TCGManagerRepository(OSMGXTCGSystemDbContext context)
        {
            _context = context;
        }

        #region Cards - GETS
        public async Task<TcgmMtgCard?> GetCardByScryfallIdAsync(string scryfallId, bool isFoil)
        {
            if (string.IsNullOrWhiteSpace(scryfallId))
                throw new ArgumentException("Scryfall ID cannot be null or empty.", nameof(scryfallId));

            return await _context.TcgmMtgCards
                .FirstOrDefaultAsync(c =>
                    c.CScryfallId.Equals(scryfallId, StringComparison.OrdinalIgnoreCase) &&
                    c.CIsFoil == isFoil
                    );
        }
        #endregion

        #region Cards - CREATES/UPDATES
        public async Task<TcgmMtgCard?> CreateOrUpdateCardAsync(TcgmMtgCard card)
        {
            if (card == null)
                throw new ArgumentNullException(nameof(card));

            var existingCard = await _context.TcgmMtgCards
                .FirstOrDefaultAsync(c =>
                    c.CScryfallId == card.CScryfallId &&
                    c.CIsFoil == card.CIsFoil &&
                    c.CSetId == card.CSetId);  // Include set as extra safety

            if (existingCard != null)
            {
                // Update existing card
                existingCard.CRarity = card.CRarity;
                existingCard.CImageUrl = card.CImageUrl;
                existingCard.CCardMarkerUrl = card.CCardMarkerUrl;
                // Add your quantity/price logic here if those properties exist

                await _context.SaveChangesAsync();
                return existingCard;
            }
            else
            {
                // Create new card
                _context.TcgmMtgCards.Add(card);
                await _context.SaveChangesAsync();
                return card;
            }
        }

        #endregion

        #region Sets - GETS

        public async Task<TcgmMtgCardSet> GetSetByCodeAsync(string setCode)
        {
            if (string.IsNullOrWhiteSpace(setCode))
                throw new ArgumentException("Set code cannot be null or empty.", nameof(setCode));

            return await _context.TcgmMtgCardSets
                .FirstOrDefaultAsync(s => s.CsSetCode.Equals(setCode, StringComparison.OrdinalIgnoreCase));
        }

        public async Task<TcgmMtgCardSet> GetSetByScryfallIdAsync(string scryfallId)
        {
            if (string.IsNullOrWhiteSpace(scryfallId))
                throw new ArgumentException("Scryfall ID cannot be null or empty.", nameof(scryfallId));

            return await _context.TcgmMtgCardSets
                .FirstOrDefaultAsync(s => s.CsScryfallId.Equals(scryfallId, StringComparison.OrdinalIgnoreCase));
        }

        #endregion

        #region Sets - CREATES/UPDATES
        public async Task<TcgmMtgCardSet> CreateSetAsync(TcgmMtgCardSet set)
        {
            if (set == null)
                throw new ArgumentNullException(nameof(set));

            _context.TcgmMtgCardSets.Add(set);
            await _context.SaveChangesAsync();
            return set;
        }
       
        #endregion


    }
}
