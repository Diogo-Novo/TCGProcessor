using System;
using System.Collections.Generic;

namespace TCGProcessor.Models;

public partial class TcgmUploadedFile
{
    public int UfId { get; set; }

    public int UfUploadSessionId { get; set; }

    public DateTime? UfUploadedAt { get; set; }

    public string UfBase64Data { get; set; } = null!;

    public string? UfFriendlyName { get; set; }

    public DateTime? UfValidityExpiryDate { get; set; }

    public virtual TcgmUploadSession UfUploadSession { get; set; } = null!;
}
