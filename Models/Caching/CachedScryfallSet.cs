using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Scryfall.API.Models;

namespace TCGProcessor.Models.Cache
{
    public class CachedScryfallSet
    {
        public Guid Id { get; set; }
        public Guid ScryfallId { get; set; }
        public string SetCode { get; set; } = string.Empty;
        public Set? SetData { get; set; }
        public DateTime CachedAt { get; set; } = DateTime.UtcNow;
        public bool IsFound { get; set; }
        public string? Error { get; set; }
        public bool IsExpired() => DateTime.UtcNow - CachedAt > TimeSpan.FromDays(7);
    }
}
