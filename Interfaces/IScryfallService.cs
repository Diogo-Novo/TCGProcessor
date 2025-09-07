using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Scryfall.API.Models;

namespace TCGProcessor.Interfaces
{
    public interface IScryfallService
    {
        Task<Card> GetCardByIdAsync(string scryfallId);
    }
}