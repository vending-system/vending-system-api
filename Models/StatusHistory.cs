using System;
using System.Collections.Generic;

namespace ApiVending.Models;

public partial class StatusHistory
{
    public int Id { get; set; }

    public string EntityType { get; set; } = null!;

    public int EntityId { get; set; }

    public string? OldStatus { get; set; }

    public string NewStatus { get; set; } = null!;

    public int? ChangedBy { get; set; }

    public DateTime? ChangedAt { get; set; }

    public string? Comment { get; set; }

    public virtual User? ChangedByNavigation { get; set; }
}
