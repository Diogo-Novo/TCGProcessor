using System;
using System.Collections.Generic;

namespace TCGProcessor.Models;

public partial class TcgmPkmCard
{
    public int CId { get; set; }

    public int CSetId { get; set; }

    public string? CCardNumber { get; set; }

    public string CCardName { get; set; } = null!;

    public string CRarity { get; set; } = null!;

    public string? CImageUrl { get; set; }

    public virtual TcgmPkmCardSet CSet { get; set; } = null!;

    public virtual ICollection<TcgmPkmMarketPriceHistory> TcgmPkmMarketPriceHistories { get; set; } = new List<TcgmPkmMarketPriceHistory>();

    public virtual ICollection<TcgmPkmSale> TcgmPkmSales { get; set; } = new List<TcgmPkmSale>();

    public virtual ICollection<TcgmPkmStockList> TcgmPkmStockLists { get; set; } = new List<TcgmPkmStockList>();
}
