using System;
using System.Collections.Generic;

namespace TCGProcessor.Models;

public partial class TcgmMtgStockList
{
    public int SlId { get; set; }

    public int SlCardId { get; set; }

    public decimal SlPurchasePrice { get; set; }

    public DateTime SlDateAcquired { get; set; }

    public virtual TcgmMtgCard SlCard { get; set; } = null!;
}
