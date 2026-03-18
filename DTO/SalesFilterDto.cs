using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiVending.DTO
{
    public class SalesFilterDto
    {
        public int? MachineId { get; set; }
        public int? ProductId { get; set; }
        public int? CompanyId { get; set; }
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
        public int? PaymentTypeId { get; set; }
    }
}