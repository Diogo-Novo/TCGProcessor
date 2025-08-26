using System;
using System.Collections.Generic;

namespace TCGProcessor.Models;

public partial class PsPricingSheet
{
    public int PsId { get; set; }

    public string? PsSheetName { get; set; }

    public int PsCreatedBy { get; set; }

    public decimal? PsTotalSellValue { get; set; }

    public decimal? PsTotalCashValue { get; set; }

    public decimal? PsTotalTradeValue { get; set; }

    public int? PsItemCount { get; set; }

    public int? PsSheetStatus { get; set; }

    public string? PsSheetNotes { get; set; }

    public DateTime? PsLastUpdated { get; set; }

    public DateTime? PsCreatedIn { get; set; }

    public sbyte? PsIsCash { get; set; }

    public decimal? PsActualAmountPaid { get; set; }

    public string? PsQuickAccessReference { get; set; }

    public virtual ICollection<PsPricingSheetItem> PsPricingSheetItems { get; set; } =
        new List<PsPricingSheetItem>();

    public virtual PsPricingSheetStatus? PsSheetStatusNavigation { get; set; }
}
