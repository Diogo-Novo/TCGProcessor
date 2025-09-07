using System;
using System.Collections.Generic;

namespace TCGProcessor.Models;

public partial class TcgmPkmStockList
{
    public int SlId { get; set; }

    public int SlCardId { get; set; }

    public int SlQuantity { get; set; }

    public decimal SlPurchasePriceEur { get; set; }

    public decimal SlPurchasePriceGbp { get; set; }

    public DateTime SlDateAcquired { get; set; }

    public virtual TcgmPkmCard SlCard { get; set; } = null!;
}
