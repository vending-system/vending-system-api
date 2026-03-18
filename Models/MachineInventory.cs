using System;
using System.Collections.Generic;

namespace ApiVending.Models;

public partial class MachineInventory
{
    public int MachineId { get; set; }

    public int ProductId { get; set; }

    public int Quantity { get; set; }

    public DateTime? LastUpdated { get; set; }

    public virtual VendingMachine Machine { get; set; } = null!;

    public virtual Product Product { get; set; } = null!;
}
