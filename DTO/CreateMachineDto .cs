using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ApiVending.DTO
{
    public class CreateMachineDto
    {
        [Required]
        public string? Name { get; set; }
        
        [Required]
        public string? SerialNumber { get; set; }
        
        [Required]
        public string? InventoryNumber { get; set; }
        
        [Required]
        public string? ModelName { get; set; }
        
        [Required]
        public string? Manufacturer { get; set; }
        
        public string? Country { get; set; }
        
        [Required]
        public int StatusId { get; set; }
        
        [Required]
        public int CompanyId { get; set; }
        
        [Required]
        public string? LocationAddress { get; set; }
        
        [Required]
        public DateTime CommissioningDate { get; set; }
        
        public string? ModemId { get; set; }
        
        public DateTime ManufactureDate { get; set; }
        public DateTime? LastCalibrationDate { get; set; }
        public int? CalibrationIntervalMonths { get; set; }
        
        [Range(1, int.MaxValue)]
        public int? ResourceHoursTotal { get; set; }
        [Range(1, 20)]
        public int? ServiceTimeHours { get; set; }
        public decimal? CurrentCash { get; set; }
        public decimal? TotalRevenue { get; set; }
    }
}