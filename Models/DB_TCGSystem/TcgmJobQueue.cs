using System;
using System.Collections.Generic;

namespace TCGProcessor.Models;

public partial class TcgmJobQueue
{
    public int JqId { get; set; }

    public string JqJobType { get; set; } = null!;

    public string JqStatus { get; set; } = null!;

    public int JqTotalEntries { get; set; }

    public int JqProcessedEntries { get; set; }

    public string? JqErrorMessage { get; set; }

    public DateTime? JqStartedAt { get; set; }

    public DateTime? JqCompletedAt { get; set; }

    public DateTime JqCreatedAt { get; set; }

    public DateTime JqUpdatedAt { get; set; }

    public int? JqResultId { get; set; }

    public virtual TcgmJobQueueDatum? TcgmJobQueueDatum { get; set; }
}
