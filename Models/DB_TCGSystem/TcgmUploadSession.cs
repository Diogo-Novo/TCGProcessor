using System;
using System.Collections.Generic;

namespace TCGProcessor.Models;

public partial class TcgmUploadSession
{
    public int UsId { get; set; }

    public string UsUniqueKey { get; set; } = null!;

    public string UsTwoFacode { get; set; } = null!;

    public DateTime UsExpiresAt { get; set; }

    public int? UsAttemptCount { get; set; }

    public bool? UsIsCompleted { get; set; }

    public DateTime? UsCreatedAt { get; set; }

    public DateTime? UsCompletedAt { get; set; }

    public int UsProcessedBy { get; set; }

    public virtual ICollection<TcgmUploadedFile> TcgmUploadedFiles { get; set; } = new List<TcgmUploadedFile>();
}
