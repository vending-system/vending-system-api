using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiVending.DTO
{
    public class CreateCompanyDto
    {
        public string Name { get; set; } = string.Empty;
        public string? Inn { get; set; }
        public string? Address { get; set; }
    }
}