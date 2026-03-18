using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiVending.DTO
{
    public class CreateServiceTaskDto
    {
        public int MachineId { get; set; }
        public int? AssignedUserId { get; set; }
        public string TaskType { get; set; } = string.Empty;
        public int Priority { get; set; } = 3;
        public DateTime ScheduledDate { get; set; }
        public string? Description { get; set; }
        public int TravelTimeMinutes { get; set; } = 60;
    }
}