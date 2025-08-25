using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Scryfall.API.Models;
namespace TCGProcessor.Models
{
    public class EnrichedMTGCard
    {
        public ManaBoxCardCsvRecord OriginalCard { get; set; }
        public Card ScryfallData { get; set; }
        public string Error { get; set; }
    }
}