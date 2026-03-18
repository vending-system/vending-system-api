using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiVending.DTO
{
    public class GrafikiSalesDto
    {
        public DateOnly Date { get; set; }
        public string DayName { get; set; } = string.Empty;
        public decimal TotalAmount { get; set; }
        public int TotalQuantity { get; set; }
        public int TransactionsCount { get; set; }
    }
}