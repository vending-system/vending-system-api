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
        public DateOnly? LastCalibrationDate { get; set; }
        public DateOnly? NextCalibrationDate { get; set; }
        public int? CalibrationIntervalMonths { get; set; }
        public string? InventoryNumber { get; set; } 
        public string? Manufacturer { get; set; } 
        public string? Country { get; set; }
        public decimal? CurrentCash { get; set; }   
        public DateOnly ManufactureDate { get; set; }
        public int? ResourceHoursTotal { get; set; }
        public int? ServiceTimeHours { get; set; }
    }
}