using System;
using System.Collections.Generic;

namespace TCGProcessor.Models;

public partial class TcgmMtgMarketPriceHistory
{
    public int MphId { get; set; }

    public int MphCardId { get; set; }

    public decimal? MphPriceUsd { get; set; }

    public decimal? MphPriceUsdFoil { get; set; }

    public decimal? MphPriceUsdEtched { get; set; }

    public decimal? MphPriceEur { get; set; }

    public decimal? MphPriceEurFoil { get; set; }

    public decimal? MphPriceEurEtched { get; set; }

    public decimal? MphPriceGbp { get; set; }

    public decimal? MphPriceGbpFoil { get; set; }

    public decimal? MphPriceGbpEtched { get; set; }

    public DateOnly MphDate { get; set; }

    public virtual TcgmMtgCard MphCard { get; set; } = null!;
}
