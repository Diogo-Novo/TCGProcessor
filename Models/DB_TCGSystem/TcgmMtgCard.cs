using System;
using System.Collections.Generic;

namespace TCGProcessor.Models;

public partial class TcgmMtgCard
{
    public int CId { get; set; }

    public int CSetId { get; set; }

    public string? CCardNumber { get; set; }

    public string CCardName { get; set; } = null!;

    public string CRarity { get; set; } = null!;

    public string? CImageUrl { get; set; }

    public string? CCardMarkerUrl { get; set; }

    public bool CIsFoil { get; set; }

    public string? CScryfallId { get; set; }

    public virtual TcgmMtgCardSet CSet { get; set; } = null!;

    public virtual ICollection<TcgmMtgMarketPriceHistory> TcgmMtgMarketPriceHistories { get; set; } = new List<TcgmMtgMarketPriceHistory>();

    public virtual ICollection<TcgmMtgSale> TcgmMtgSales { get; set; } = new List<TcgmMtgSale>();

    public virtual ICollection<TcgmMtgStockList> TcgmMtgStockLists { get; set; } = new List<TcgmMtgStockList>();
}
