using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiVending.DTO
{
    public class MachineListDto
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? SerialNumber { get; set; }
        public string? ModelName { get; set; }
        public MachineStatusDto?  Status { get; set; }
        public CompanyDto?  Company { get; set; }
        public string?  LocationAddress { get; set; }
        public DateOnly  CommissioningDate { get; set; }
        public string?  ModemId { get; set; }
    }
}