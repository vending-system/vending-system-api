using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiVending.DTO
{
    public class TopProductDto
    {
    public int ProductId { get; set; }
    public string ProductName { get; set; } = "";
    public int TotalQuantity { get; set; }
    public decimal TotalMoney { get; set; }
    public int TransactionsCount { get; set; }
    }
}