using System;
using System.Collections.Generic;

namespace TCGProcessor.Models;

public partial class TcgmMtgCardSet
{
    public int CsId { get; set; }

    public string CsSetName { get; set; } = null!;

    public string CsSetCode { get; set; } = null!;

    public string? CsScryfallId { get; set; }

    public DateOnly? CsReleaseDate { get; set; }

    public string? CsSetIconSvgurl { get; set; }

    public virtual ICollection<TcgmMtgCard> TcgmMtgCards { get; set; } = new List<TcgmMtgCard>();
}
