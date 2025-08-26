using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Scryfall.API.Models;

namespace TCGProcessor.Models.Cache
{
    public class CachedScryfallCard
    {
        public Guid Id { get; set; }
        public Guid ScryfallId { get; set; }
        public Card? CardData { get; set; }
        public DateTime CachedAt { get; set; } = DateTime.UtcNow;
        public bool IsFound { get; set; }
        public string? Error { get; set; }

        public bool IsExpired() => CachedAt.Date != DateTime.UtcNow.Date;
    }
}
