using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiVending.DTO
{
    public class EmployeeScheduleDto
    {
        public int UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public List<CalendarEventDto> Tasks { get; set; } = [];
        public int TotalHours { get; set; }
        public string Specialization { get; set; } = string.Empty;
    }
}