using System;
using System.Collections.Generic;

namespace TCGProcessor.Models;

public partial class PsTemporaryPricingItem
{
    public int TpiId { get; set; }

    public string? TpiProductName { get; set; }

    public string? TpiProductCategoryName { get; set; }

    public decimal? TpiSellPrice { get; set; }

    public decimal? TpiCashPrice { get; set; }

    public decimal? TpiTradePrice { get; set; }

    public string? TpiWeBuyId { get; set; }

    public DateTime? TpiCreatedIn { get; set; }

    public bool? TpiImported { get; set; }

    public int? TpiGroupId { get; set; }

    public virtual PsTemporaryPricingItemGroup? TpiGroup { get; set; }
}
