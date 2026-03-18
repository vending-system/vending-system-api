using System;
using System.Collections.Generic;

namespace ApiVending.Models;

public partial class File
{
    public int Id { get; set; }

    public string EntityType { get; set; } = null!;

    public int EntityId { get; set; }

    public string FilePath { get; set; } = null!;

    public string? FileName { get; set; }

    public string? MimeType { get; set; }

    public int? UploadedBy { get; set; }

    public DateTime? UploadedAt { get; set; }

    public virtual User? UploadedByNavigation { get; set; }
}
