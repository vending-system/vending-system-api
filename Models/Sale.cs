using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ApiVending.Models;

[Table("sales")]
public partial class Sale
{
    [Key]
    public int Id { get; set; }

    public int? MachineId { get; set; }

    public int? ProductId { get; set; }

    public int Quantity { get; set; }

    public decimal Amount { get; set; }//общая сумма продажи

    public int? PaymentTypeId { get; set; }
    public DateTime SaleDate { get; set; }
    public DateTime? CreatedAt { get; set; }
    [ForeignKey("MachineId")]
    public virtual VendingMachine? Machine { get; set; }
    [ForeignKey("PaymentTypeId")]
    public virtual PaymentType? PaymentType { get; set; }
    [ForeignKey("ProductId")]
    public virtual Product? Product { get; set; }
}
