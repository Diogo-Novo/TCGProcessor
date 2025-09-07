using System;
using System.Collections.Generic;

namespace TCGProcessor.Models;

public partial class TcgmMtgImportConfig
{
    public int IcId { get; set; }

    public decimal IcMinPrice { get; set; }

    public bool IcIncludeCommon { get; set; }

    public bool IcIncludeUncommon { get; set; }

    public bool? IcIncludeRare { get; set; }

    public bool? IcIncludeMythic { get; set; }
}
