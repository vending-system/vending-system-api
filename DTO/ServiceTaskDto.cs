using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiVending.DTO
{
    public class ServiceTaskDto
    {
        public int Id { get; set; }
        public int MachineId { get; set; }
        public string MachineName { get; set; } = string.Empty;
        public string MachineLocation { get; set; } = string.Empty;
        public string? AssignedUserName { get; set; }
        public int AssignedUserId { get; set; }
        public string TaskType { get; set; } = string.Empty; // Плановое, Авария, Ремонт
        public string Status { get; set; } = string.Empty; 
        public int? Priority { get; set; } // 1-авария, 2-высокий, 3-нормальный
        public DateTime ScheduledDate { get; set; }
        public DateTime? ActualCompletionDate { get; set; }
        public string? Description { get; set; }
        public int? TravelTimeMinutes { get; set; }
        public string? CancellationReason { get; set; }      
        public string ColorCode { get; set; } = string.Empty; // green, yellow, red
    }
}