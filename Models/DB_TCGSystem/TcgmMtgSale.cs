using System;
using System.Collections.Generic;

namespace TCGProcessor.Models;

public partial class TcgmMtgSale
{
    public int SId { get; set; }

    public int SCardId { get; set; }

    public int SQuantity { get; set; }

    public decimal SSalePrice { get; set; }

    public decimal? STotalAmount { get; set; }

    public DateTime SSaleDate { get; set; }

    public virtual TcgmMtgCard SCard { get; set; } = null!;
}
