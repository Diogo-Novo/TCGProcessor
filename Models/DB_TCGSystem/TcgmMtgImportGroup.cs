using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TCGProcessor.Models.DB_TCGSystem
{
    public class TcgmMtgImportGroup
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public int CreatedBy { get; set; }

        public int? CurrentPricingSheetId { get; set; }
        // Navigation properties
        public virtual ICollection<TcgmMtgCardImport> TcgmMtgCardImports { get; set; } = new List<TcgmMtgCardImport>();


    }
}