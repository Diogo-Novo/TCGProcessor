using Microsoft.Extensions.Caching.Memory;
using TCGProcessor.Models.Cache;
using TCGProcessor.Interfaces;
using Scryfall.API;
using Scryfall.API.Models;
using System.Collections.Concurrent;
using TCGProcessor.Data;
using Microsoft.EntityFrameworkCore;

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

        public ScryfallService(IMemoryCache cache, ILogger<ScryfallService> logger, OSMGXProcessorDbContext dbProcessorContext)
        {
            _scryfallClient = new ScryfallClient();
            _cache = cache;
            _logger = logger;
            _rateLimitSemaphore = new SemaphoreSlim(8, 8); // Allow 8 concurrent requests
            _dbProcessorContext = dbProcessorContext;
        }

        public async Task<Card?> GetCardById(string scryfallId)
{
    var scryfallGuid = new Guid(scryfallId);
    var cacheKey = $"scryfall_id_{scryfallId}";

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
    var dbCached = await _dbProcessorContext.ScryfallCache
        .FirstOrDefaultAsync(c => c.ScryfallId == scryfallGuid);
    
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
            _dbProcessorContext.ScryfallCache.Update(dbCached);
        }
        else
        {
            // Add new record
            _dbProcessorContext.ScryfallCache.Add(cached);
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