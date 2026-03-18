using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiVending.DTO
{
    public class UpdateServiceTaskDto
    {
        public int? AssignedUserId { get; set; }
        public string? TaskType { get; set; }
        public string? Status { get; set; }
        public int? Priority { get; set; }
        public DateTime? ScheduledDate { get; set; }
        public string? Description { get; set; }
        public string? CancellationReason { get; set; }
    }
}