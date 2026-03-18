using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace ApiVending.Models;

public partial class ServiceTask
{
    public int Id { get; set; }

    public int MachineId { get; set; }

    public int? AssignedUserId { get; set; }

    public string TaskType { get; set; } = null!;

    public string? Status { get; set; }

    public int? Priority { get; set; }

    public DateTime ScheduledDate { get; set; }
    public DateTime? ActualCompletionDate { get; set; }

    public int? CounterValueBefore { get; set; }

    public int? CounterValueAfter { get; set; }

    public int? TravelTimeMinutes { get; set; }

    public string? ReportText { get; set; }

    public string? ReportFileUrl { get; set; }

    public string? CancellationReason { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual User? AssignedUser { get; set; }

    public virtual VendingMachine? Machine { get; set; }
}
