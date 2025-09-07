using System.Collections.Concurrent;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Scryfall.API;
using Scryfall.API.Models;
using TCGProcessor.Data;
using TCGProcessor.Interfaces;
using TCGProcessor.Models;
using TCGProcessor.Models.Cache;
using TCGProcessor.Repositories;

namespace TCGProcessor.Services
{
    public class ScryfallService : IScryfallService
    {
        private readonly ScryfallClient _scryfallClient;
        private readonly IMemoryCache _cache;
        private readonly ILogger<ScryfallService> _logger;
        private readonly TimeSpan _cacheLifetime = TimeSpan.FromHours(6);
        private readonly SemaphoreSlim _rateLimitSemaphore;
        private readonly ConcurrentDictionary<string, DateTime> _lastRequestTimes = new();
        private readonly OSMGXProcessorDbContext _dbProcessorContext;
        private readonly TCGManagerRepository _tcgManagerRepository;


        public ScryfallService(
            IMemoryCache cache,
            ILogger<ScryfallService> logger,
            OSMGXProcessorDbContext dbProcessorContext,
            TCGManagerRepository tCGManagerRepository
        )
        {
            _scryfallClient = new ScryfallClient();
            _cache = cache;
            _logger = logger;
            _rateLimitSemaphore = new SemaphoreSlim(8, 8); // Allow 8 concurrent requests
            _dbProcessorContext = dbProcessorContext;
            _tcgManagerRepository = tCGManagerRepository;
        }

        public async Task<Card?> GetCardByIdAsync(string scryfallId)
        {
            var scryfallGuid = new Guid(scryfallId);
            var cacheKey = $"scryfall_card_id_{scryfallId}";

            // 1. Check in-memory cache
            if (_cache.TryGetValue(cacheKey, out CachedScryfallCard? cachedCard))
            {
                if (!cachedCard!.IsExpired())
                {
                    _logger.LogDebug("Memory cache hit for Scryfall ID: {ScryfallId}", scryfallId);
                    return cachedCard.IsFound ? cachedCard.CardData : null;
                }
            }

            // 2. Check database cache - query by ScryfallId
            var dbCached = await _dbProcessorContext.ScryfallCardCache.FirstOrDefaultAsync(c =>
                c.ScryfallId == scryfallGuid
            );

            if (dbCached != null && !dbCached.IsExpired())
            {
                _logger.LogDebug("Database cache hit for Scryfall ID: {ScryfallId}", scryfallId);
                _cache.Set(cacheKey, dbCached, _cacheLifetime);
                return dbCached.IsFound ? dbCached.CardData : null;
            }

            await _rateLimitSemaphore.WaitAsync();

            try
            {
                await ApplyRateLimit();

                var card = _scryfallClient.Cards.GetById(scryfallGuid);

                var cached = new CachedScryfallCard
                {
                    Id = Guid.NewGuid(),
                    ScryfallId = scryfallGuid,
                    CardData = card,
                    CachedAt = DateTime.UtcNow,
                    IsFound = card != null
                };

                _cache.Set(cacheKey, cached, _cacheLifetime);

                // Only cache successful results in database
                if (dbCached != null)
                {
                    // Update existing record
                    dbCached.CardData = cached.CardData;
                    dbCached.CachedAt = cached.CachedAt;
                    dbCached.IsFound = cached.IsFound;
                    dbCached.Error = null; // Clear any previous error
                    _dbProcessorContext.ScryfallCardCache.Update(dbCached);
                }
                else
                {
                    // Add new record
                    _dbProcessorContext.ScryfallCardCache.Add(cached);
                }

                await _dbProcessorContext.SaveChangesAsync();

                _logger.LogDebug("Cached Scryfall card: {ScryfallId}", scryfallId);
                return card;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to fetch card with ID: {ScryfallId}", scryfallId);

                // Only cache error in memory for short period, NOT in database
                var errorCached = new CachedScryfallCard
                {
                    Id = Guid.NewGuid(),
                    ScryfallId = scryfallGuid,
                    CardData = null,
                    CachedAt = DateTime.UtcNow,
                    IsFound = false,
                    Error = ex.Message
                };

                _cache.Set(cacheKey, errorCached, TimeSpan.FromMinutes(5)); // Short-lived memory cache only

                return null;
            }
            finally
            {
                _rateLimitSemaphore.Release();
            }
        }

        public async Task<Set?> GetSetByIdAsync(string scryfallId)
        {
            var scryfallGuid = new Guid(scryfallId);
            var cacheKey = $"scryfall_set_id_{scryfallId}";

            // 1. Check in-memory cache
            if (_cache.TryGetValue(cacheKey, out CachedScryfallSet? cachedSet))
            {
                if (!cachedSet!.IsExpired())
                {
                    _logger.LogDebug("Memory cache hit for SET Scryfall ID: {ScryfallId}", scryfallId);
                    return cachedSet.IsFound ? cachedSet.SetData : null;
                }
            }

            // 2. Check database cache - query by ScryfallId
            var dbCached = await _dbProcessorContext.ScryfallSetCache.FirstOrDefaultAsync(c =>
                c.ScryfallId == scryfallGuid
            );

            if (dbCached != null && !dbCached.IsExpired())
            {
                _logger.LogDebug("Database cache hit for SET Scryfall ID: {ScryfallId}", scryfallId);
                _cache.Set(cacheKey, dbCached, _cacheLifetime);
                return dbCached.IsFound ? dbCached.SetData : null;
            }

            await _rateLimitSemaphore.WaitAsync();

            try
            {
                await ApplyRateLimit();

                var sets = _scryfallClient.Sets.GetAll();
                var set = sets.Data.FirstOrDefault(s => s.Id == scryfallGuid);
                var cached = new CachedScryfallSet
                {
                    Id = Guid.NewGuid(),
                    ScryfallId = scryfallGuid,
                    SetData = set,
                    CachedAt = DateTime.UtcNow,
                    IsFound = set != null
                };

                _cache.Set(cacheKey, cached, _cacheLifetime);

                // Only cache successful results in database
                if (dbCached != null)
                {
                    // Update existing record
                    dbCached.SetData = cached.SetData;
                    dbCached.CachedAt = cached.CachedAt;
                    dbCached.IsFound = cached.IsFound;
                    dbCached.Error = null; // Clear any previous error
                    _dbProcessorContext.ScryfallSetCache.Update(dbCached);
                }
                else
                {
                    // Add new record
                    _dbProcessorContext.ScryfallSetCache.Add(cached);
                }

                await _dbProcessorContext.SaveChangesAsync();

                _logger.LogDebug("Cached Scryfall card: {ScryfallId}", scryfallId);
                return set;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to fetch card with ID: {ScryfallId}", scryfallId);

                // Only cache error in memory for short period, NOT in database
                var errorCached = new CachedScryfallCard
                {
                    Id = Guid.NewGuid(),
                    ScryfallId = scryfallGuid,
                    CardData = null,
                    CachedAt = DateTime.UtcNow,
                    IsFound = false,
                    Error = ex.Message
                };

                _cache.Set(cacheKey, errorCached, TimeSpan.FromMinutes(5)); // Short-lived memory cache only

                return null;
            }
            finally
            {
                _rateLimitSemaphore.Release();
            }

        }

        public async Task<Set?> GetSetBySetCodeAsync(string setCode)
        {
            var cacheKey = $"scryfall_set_code_{setCode}";

            // 1. Check in-memory cache
            if (_cache.TryGetValue(cacheKey, out CachedScryfallSet? cachedSet))
            {
                if (!cachedSet!.IsExpired())
                {
                    _logger.LogDebug("Memory cache hit for SET CODE: {ScryfallId}", setCode);
                    return cachedSet.IsFound ? cachedSet.SetData : null;
                }
            }

            // 2. Check database cache - query by ScryfallId
            var dbCached = await _dbProcessorContext.ScryfallSetCache.FirstOrDefaultAsync(c =>
                c.SetCode == setCode
            );

            if (dbCached != null && !dbCached.IsExpired())
            {
                _logger.LogDebug("Database cache hit for SET CODE ID: {ScryfallId}", setCode);
                _cache.Set(cacheKey, dbCached, _cacheLifetime);
                return dbCached.IsFound ? dbCached.SetData : null;
            }

            await _rateLimitSemaphore.WaitAsync();

            try
            {
                await ApplyRateLimit();


                var set = await _scryfallClient.Sets.GetByCodeAsync(setCode);
                var cached = new CachedScryfallSet
                {
                    Id = Guid.NewGuid(),
                    ScryfallId = set.Id,
                    SetCode = set.Code,
                    SetData = set,
                    CachedAt = DateTime.UtcNow,
                    IsFound = set != null
                };

                _cache.Set(cacheKey, cached, _cacheLifetime);

                // Only cache successful results in database
                if (dbCached != null)
                {
                    // Update existing record
                    dbCached.SetData = cached.SetData;
                    dbCached.CachedAt = cached.CachedAt;
                    dbCached.IsFound = cached.IsFound;
                    dbCached.Error = null; // Clear any previous error
                    _dbProcessorContext.ScryfallSetCache.Update(dbCached);
                }
                else
                {
                    // Add new record
                    _dbProcessorContext.ScryfallSetCache.Add(cached);
                }

                await _dbProcessorContext.SaveChangesAsync();

                _logger.LogDebug("Cached Scryfall set: {ScryfallId}", set.Id);
                return set;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to fetch set with Code: {ScryfallId}", setCode);

                // Only cache error in memory for short period, NOT in database
                var errorCached = new CachedScryfallSet
                {
                    Id = Guid.NewGuid(),
                    ScryfallId = Guid.Empty,
                    SetCode = setCode,
                    SetData = null,
                    CachedAt = DateTime.UtcNow,
                    IsFound = false,
                    Error = ex.Message
                };

                _cache.Set(cacheKey, errorCached, TimeSpan.FromMinutes(5)); // Short-lived memory cache only

                return null;
            }
            finally
            {
                _rateLimitSemaphore.Release();
            }

        }

        public async Task<TcgmMtgCard?> ConvertScryfallCardToTcgmCard(Card scryfallCard, bool isFoil)
        {
            if (scryfallCard == null)
                return null;

            TcgmMtgCard? existingCard = await _tcgManagerRepository.GetCardByScryfallIdAsync(scryfallCard.Id.ToString(),isFoil);
            if (existingCard != null)
                return existingCard;

            
            TcgmMtgCardSet? tcgmMtgCardSet = await _tcgManagerRepository.GetSetByCodeAsync(scryfallCard.Set);

            Set? cardSet = await GetSetBySetCodeAsync(scryfallCard.Set);

            // Map Scryfall Card to TcgmMtgCard
            var tcgmCard = new TcgmMtgCard
            {
                
            };

            return tcgmCard;
        }

        private async Task ApplyRateLimit()
        {
            const int minDelayMs = 100; // Minimum 100ms between requests
            var currentTime = DateTime.UtcNow;
            var key = "global";

            if (_lastRequestTimes.TryGetValue(key, out var lastRequest))
            {
                var timeSinceLastRequest = currentTime - lastRequest;
                var remainingDelay = TimeSpan.FromMilliseconds(minDelayMs) - timeSinceLastRequest;

                if (remainingDelay > TimeSpan.Zero)
                {
                    await Task.Delay(remainingDelay);
                }
            }

            _lastRequestTimes[key] = DateTime.UtcNow;
        }
    }
}
