using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ApiVending.Models;
[Table("vending_machines")]
public partial class VendingMachine
{
    [Key]
    public int Id { get; set; }

    public string? Name { get; set; }

    [Required]
    public string SerialNumber { get; set; } = null!;
    [Required]
    public string InventoryNumber { get; set; } = null!;
    [Required]
    public string ModelName { get; set; } = null!;
    [Required]
    public string Manufacturer { get; set; } = null!;

    public string? Country { get; set; }

    public int? StatusId { get; set; }

    public int? PaymentTypeId { get; set; }

    public int? CompanyId { get; set; }

    public DateOnly ManufactureDate { get; set; }

    public DateOnly CommissioningDate { get; set; }

    public DateOnly? LastCalibrationDate { get; set; }

    public DateOnly? NextCalibrationDate { get; set; }

    public DateOnly? LastInventoryDate { get; set; }

    public DateOnly? SystemEntryDate { get; set; }

    public int? CalibrationIntervalMonths { get; set; }

    public int? ResourceHoursTotal { get; set; }

    public int? OperatingHoursCurrent { get; set; }

    public int? ServiceTimeHours { get; set; }

    public decimal? TotalRevenue { get; set; }

    public decimal? CurrentCash { get; set; }

    public string? ModemId { get; set; }

    public string LocationAddress { get; set; } = null!;

    public decimal? Latitude { get; set; }

    public decimal? Longitude { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }
    [ForeignKey("CompanyId")]
    public virtual Company? Company { get; set; }

    public virtual ICollection<MachineInventory> MachineInventories { get; set; } = new List<MachineInventory>();

    public virtual ICollection<Notification> Notifications { get; set; } = new List<Notification>();

    public virtual PaymentType? PaymentType { get; set; }

    public virtual ICollection<Sale> Sales { get; set; } = new List<Sale>();

    public virtual ICollection<ServiceTask> ServiceTasks { get; set; } = new List<ServiceTask>();
    [ForeignKey("StatusId")]
    public virtual MachineStatus? Status { get; set; }

}
