using System;
using System.Collections.Generic;

namespace TCGProcessor.Models;

public partial class TcgmTillBasket
{
    public int BId { get; set; }

    public DateTime BCreatedAt { get; set; }

    public DateTime BUpdatedAt { get; set; }

    public int BCreatedBy { get; set; }

    public int? BClosedBy { get; set; }

    public DateTime? BClosedAt { get; set; }

    public string? BNotes { get; set; }

    public string BStatus { get; set; } = null!;

    public decimal? BTotalAmount { get; set; }

    public virtual ICollection<TcgmTillBasketItem> TcgmTillBasketItems { get; set; } = new List<TcgmTillBasketItem>();
}
