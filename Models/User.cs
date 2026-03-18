using System;
using System.Collections.Generic;

namespace ApiVending.Models;

public partial class User
{
    public int Id { get; set; }

    public string Username { get; set; } = null!;

    public string PasswordHash { get; set; } = null!;

    public string Fio { get; set; } = null!;

    public string? Email { get; set; }

    public string? Phone { get; set; }

    public int? RoleId { get; set; }

    public int? CompanyId { get; set; }

    public bool? IsActive { get; set; }

    public string? PhotoUrl { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual Company? Company { get; set; }

    public virtual ICollection<File> Files { get; set; } = new List<File>();

    public virtual Role? Role { get; set; }

    public virtual ICollection<ServiceTask> ServiceTasks { get; set; } = new List<ServiceTask>();

    public virtual ICollection<StatusHistory> StatusHistories { get; set; } = new List<StatusHistory>();
}
