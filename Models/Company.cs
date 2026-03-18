using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ApiVending.Models;
[Table("companies")]
public partial class Company
{
    [Key]
    public int Id { get; set; }
    [Required]
    public string Name { get; set; } = null!;
    public string? Inn { get; set; }
    public string? Address { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual ICollection<User> Users  { get; set; } = new List<User>();

    public virtual ICollection<VendingMachine> VendingMachines { get; set; } = new List<VendingMachine>();
}
