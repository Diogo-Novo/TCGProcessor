using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Scryfall.API.Models;
using TCGProcessor.Data;
using TCGProcessor.Models;
using TCGProcessor.Models.Cache;

namespace TCGProcessor.Repositories
{
    public class PricingSheetRepository
    {
        private readonly OsMgxPricingSystemContext _context;

        public PricingSheetRepository(OsMgxPricingSystemContext context)
        {
            _context = context;
        }

        public async Task<bool> PricingSheetExists(int id)
        {
            return await _context.PsPricingSheets.AnyAsync(e => e.PsId == id);
        }

        public List<PsTemporaryPricingItem> GetItems()
        {
            return _context.PsTemporaryPricingItems.Take(10).ToList();
        }

        internal PsPricingSheet GetPricingSheetById(int pricingSheetId)
        {
            return _context
                .PsPricingSheets.Include(ps => ps.PsPricingSheetItems)
                .FirstOrDefault(ps => ps.PsId == pricingSheetId);
        }

        public async Task UpdatePricingSheetAsync(PsPricingSheet pricingSheet)
        {
            if (pricingSheet == null)
                throw new ArgumentNullException(nameof(pricingSheet));

            // If the entity is already being tracked, EF will detect changes automatically
            _context.PsPricingSheets.Update(pricingSheet);
            await _context.SaveChangesAsync();
        }
    }
}
