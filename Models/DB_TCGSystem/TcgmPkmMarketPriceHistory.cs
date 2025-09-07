using System;
using System.Collections.Generic;

namespace TCGProcessor.Models;

public partial class TcgmPkmMarketPriceHistory
{
    public int MphId { get; set; }

    public int MphCardId { get; set; }

    public decimal MphPrice { get; set; }

    public DateOnly MphDate { get; set; }

    public virtual TcgmPkmCard MphCard { get; set; } = null!;
}
