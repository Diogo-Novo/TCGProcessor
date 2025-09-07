using System;
using System.Collections.Generic;

namespace TCGProcessor.Models;

public partial class TcgmJobQueueDatum
{
    public int JqdJobQueueId { get; set; }

    public string JqdData { get; set; } = null!;

    public virtual TcgmJobQueue JqdJobQueue { get; set; } = null!;
}
