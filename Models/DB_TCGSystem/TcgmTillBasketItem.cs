using System;
using System.Collections.Generic;

namespace TCGProcessor.Models;

public partial class TcgmTillBasketItem
{
    public int BiId { get; set; }

    public int BiBasketId { get; set; }

    public string BiGame { get; set; } = null!;

    public int BiCardId { get; set; }

    public int BiQuantity { get; set; }

    public decimal BiPrice { get; set; }

    public virtual TcgmTillBasket BiBasket { get; set; } = null!;
}
