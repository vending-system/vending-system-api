using System;
using System.Collections.Generic;

namespace ApiVending.Models;

public partial class Notification
{
    public int Id { get; set; }

    public int? MachineId { get; set; }

    public string Type { get; set; } = null!;

    public string Title { get; set; } = null!;

    public string Message { get; set; } = null!;

    public bool? IsRead { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual VendingMachine? Machine { get; set; }
}
