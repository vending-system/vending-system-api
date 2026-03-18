using System;
using System.Collections.Generic;

namespace ApiVending.Models;

public partial class Product
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public decimal Price { get; set; }

    public int? MinStockLevel { get; set; }

    public decimal? AvgDailySales { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual ICollection<MachineInventory> MachineInventories { get; set; } = new List<MachineInventory>();

    public virtual ICollection<Sale> Sales { get; set; } = new List<Sale>();
}
