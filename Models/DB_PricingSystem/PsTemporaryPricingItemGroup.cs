using System;
using System.Collections.Generic;

namespace TCGProcessor.Models;

public partial class PsTemporaryPricingItemGroup
{
    public int TpigId { get; set; }

    public int? TpigItemCount { get; set; }

    public DateTime? TpigCreatedIn { get; set; }

    public bool? TpigImported { get; set; }

    public decimal? TpigTotalSellValue { get; set; }

    public decimal? TpigTotalCashValue { get; set; }

    public decimal? TpigTotalTradeValue { get; set; }

    public virtual ICollection<PsTemporaryPricingItem> PsTemporaryPricingItems { get; set; } = new List<PsTemporaryPricingItem>();
}
