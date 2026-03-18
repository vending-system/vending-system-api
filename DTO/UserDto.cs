using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiVending.DTO
{
    public class UserDto
    {
        public int Id {get;set;}
     public string Username { get; set; } = null!;
    public string Password { get; set; } = null!;

    public string Fio { get; set; } = null!;

    public string? Email { get; set; }

    public string? Phone { get; set; }

    public int? RoleId { get; set; }

    public int? CompanyId { get; set; }

    public bool? IsActive { get; set; }

    public string? PhotoUrl { get; set; }
    }
}