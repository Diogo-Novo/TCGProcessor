using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TCGProcessor.Models
{
    public class ManaBoxCardCsvRecord
    {
        public string BinderName { get; set; }
        public string BinderType { get; set; }
        public string Name { get; set; }
        public string SetCode { get; set; }
        public string SetName { get; set; }
        public string CollectorNumber { get; set; }
        public CardFinish Finish { get; set; }
        public string Rarity { get; set; }
        public int Quantity { get; set; }
        public string ManaBoxId { get; set; }
        public string ScryfallId { get; set; }
        public decimal PurchasePrice { get; set; }

        public bool Misprint { get; set; }
        public bool Altered { get; set; }
        public string Condition { get; set; }
        public string Language { get; set; }
        public string Currency { get; set; }
    }
     public enum CardFinish
    {
        Normal,
        Foil,
        Etched
    }
}