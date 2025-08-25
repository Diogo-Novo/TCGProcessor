using System;
using System.Collections.Generic;

namespace TCGProcessor.Models;

public partial class PsPricingSheetItem
{
    public int PsiId { get; set; }

    public string PsiItemName { get; set; } = null!;

    public string? PsiSerialNumber { get; set; }

    public decimal? PsiSaleValue { get; set; }

    public decimal? PsiCashValue { get; set; }

    public decimal? PsiTradeValue { get; set; }

    public string? PsiItemNotes { get; set; }

    public int? PsiPricingSheet { get; set; }

    public int? PsiCreatedBy { get; set; }

    public string? PsiWeBuyReference { get; set; }

    public virtual PsPricingSheet? PsiPricingSheetNavigation { get; set; }
}
