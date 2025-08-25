using System;
using System.Collections.Generic;

namespace TCGProcessor.Models;

public partial class PsPricingSheetStatus
{
    public int PssId { get; set; }

    public string? PssStatusName { get; set; }

    public sbyte? PssIsFinalized { get; set; }

    public virtual ICollection<PsPricingSheet> PsPricingSheets { get; set; } = new List<PsPricingSheet>();
}
