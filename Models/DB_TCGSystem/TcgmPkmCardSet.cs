using System;
using System.Collections.Generic;

namespace TCGProcessor.Models;

public partial class TcgmPkmCardSet
{
    public int CsId { get; set; }

    public string CsSetName { get; set; } = null!;

    public string CsSetCode { get; set; } = null!;

    public DateOnly? CsReleaseDate { get; set; }

    public virtual ICollection<TcgmPkmCard> TcgmPkmCards { get; set; } = new List<TcgmPkmCard>();
}
