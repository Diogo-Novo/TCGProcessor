using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TCGProcessor.Models.DB_TCGSystem
{
    public class TcgmMtgCardImport
    {
        public int Id { get; set; }
        public int ImportGroupId { get; set; }
        public string CardName { get; set; } = null!;
        public string SetCode { get; set; } = null!;
        public string Rarity { get; set; } = null!;
        public string Language { get; set; } = null!;
        public string Condition { get; set; } = null!;
        public int Quantity { get; set; }
        public decimal GbpPrice { get; set; }
        public string Currency { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public int CreatedBy { get; set; }

        // Navigation properties
        public virtual TcgmMtgImportGroup ImportGroup { get; set; } = null!;
    }
}