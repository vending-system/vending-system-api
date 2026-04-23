using ApiVending.DTO;
using ApiVending.Models;
using ApiVending.Services.ServerTask;
using Microsoft.EntityFrameworkCore;

public class ServiceTaskService(VendingSystemDbContext context) : IServerTask
{
    private readonly VendingSystemDbContext _context = context;

    private static string GetColorCode(ServiceTask task)
    {
        if (task.Status == "Закрыта") return "green";
        if (task.ScheduledDate < DateTime.UtcNow) return "red";
        if (task.ScheduledDate <= DateTime.UtcNow.AddDays(5)) return "yellow";
        return "blue";
    }

    private static ServiceTaskDto MapToDto(ServiceTask t) => new()
    {
        Id = t.Id,
        MachineId = t.MachineId,
        MachineName = t.Machine?.Name ?? "Неизвестно",
        MachineLocation = t.Machine?.LocationAddress ?? "",
        AssignedUserId = t.AssignedUserId ?? 0,
        AssignedUserName = t.AssignedUser?.Fio ?? "Не назначен",
        TaskType = t.TaskType ?? "",
        Status = t.Status ?? "Новая",
        Priority = t.Priority ?? 3,
        ScheduledDate = t.ScheduledDate,
        ActualCompletionDate = t.ActualCompletionDate,
        Description = t.ReportText,
        TravelTimeMinutes = t.TravelTimeMinutes ?? 60,
        CancellationReason = t.CancellationReason,
        ColorCode = GetColorCode(t)
    };

    private async Task LogStatusAsync(int entityId, string? oldStatus, string newStatus)
    {
        _context.StatusHistories.Add(new StatusHistory
        {
            EntityType = "service_task",
            EntityId = entityId,
            OldStatus = oldStatus,
            NewStatus = newStatus,
            ChangedAt = DateTime.UtcNow
        });
        await _context.SaveChangesAsync();
    }

    public async Task<List<ServiceTaskDto>> GetTasksAsync(DateTime? fromDate, DateTime? toDate,
        int? machineId, int? assignedTo, string? status, string? taskType)
    {
        var query = _context.ServiceTasks
            .Include(t => t.Machine)
            .Include(t => t.AssignedUser)
            .AsQueryable();

        if (fromDate != null) query = query.Where(t => t.ScheduledDate >= fromDate);
        if (toDate != null) query = query.Where(t => t.ScheduledDate <= toDate);
        if (machineId != null) query = query.Where(t => t.MachineId == machineId);
        if (assignedTo != null) query = query.Where(t => t.AssignedUserId == assignedTo);
        if (status != null) query = query.Where(t => t.Status == status);
        if (taskType != null) query = query.Where(t => t.TaskType == taskType);

        var tasks = await query.OrderBy(t => t.ScheduledDate).ToListAsync();
        return tasks.Select(MapToDto).ToList();
    }

    public async Task<ServiceTaskDto?> GetTaskAsync(int id)
    {
        var task = await _context.ServiceTasks
            .Include(t => t.Machine)
            .Include(t => t.AssignedUser)
            .FirstOrDefaultAsync(t => t.Id == id);

        return task == null ? null : MapToDto(task);
    }

    public async Task<ServiceTaskDto> CreateTaskAsync(CreateServiceTaskDto dto)
    {
        if (!await _context.VendingMachines.AnyAsync(m => m.Id == dto.MachineId))
            throw new KeyNotFoundException("Аппарат не найден");

        var task = new ServiceTask
        {
            MachineId = dto.MachineId,
            AssignedUserId = dto.AssignedUserId,
            TaskType = dto.TaskType,
            Status = "Новая",
            Priority = dto.Priority,
            ScheduledDate = dto.ScheduledDate,
            ReportText = dto.Description,
            TravelTimeMinutes = dto.TravelTimeMinutes,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.ServiceTasks.Add(task);
        await _context.SaveChangesAsync();
        await LogStatusAsync(task.Id, null, "Новая");

        return MapToDto(task);
    }

    public async Task StartTaskAsync(int id)
    {
        var task = await _context.ServiceTasks.FindAsync(id)
            ?? throw new KeyNotFoundException($"Задача {id} не найдена");

        if (task.Status != "Новая")
            throw new InvalidOperationException("Задачу можно начать только из статуса Новая");

        task.Status = "В работе";
        task.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
        await LogStatusAsync(task.Id, "Новая", "В работе");
    }
    public async Task CompleteTaskAsync(int id)
    {
        var task = await _context.ServiceTasks.FindAsync(id)
            ?? throw new KeyNotFoundException($"Задача {id} не найдена");

        var oldStatus = task.Status;
        task.Status = "Закрыта";
        task.ActualCompletionDate = DateTime.UtcNow;
        task.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
        await LogStatusAsync(task.Id, oldStatus, "Закрыта");
    }
    public async Task AssignTaskAsync(int taskId, int userId)
    {
        var task = await _context.ServiceTasks.FindAsync(taskId)
            ?? throw new KeyNotFoundException($"Задача {taskId} не найдена");

        task.AssignedUserId = userId;
        task.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
    }
    public async Task DeleteTaskAsync(int id)
    {
        var task = await _context.ServiceTasks.FindAsync(id)
            ?? throw new KeyNotFoundException($"Задача {id} не найдена");

        if (task.Status == "В работе")
            throw new InvalidOperationException("Задача в работе");

        _context.ServiceTasks.Remove(task);
        await _context.SaveChangesAsync(); 
        await LogStatusAsync(task.Id, task.Status, "Удалена");
    }

    public async Task UpdateTaskAsync(int id, UpdateServiceTaskDto dto)
    {
        var task = await _context.ServiceTasks.FindAsync(id)
            ?? throw new KeyNotFoundException($"Задача {id} не найдена");

        task.TaskType = dto.TaskType ?? task.TaskType;
        task.Priority = dto.Priority ?? task.Priority;
        task.ScheduledDate = dto.ScheduledDate ?? task.ScheduledDate;
        task.ReportText = dto.Description ?? task.ReportText;
        task.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
    }

    public async Task CancelTaskAsync(int id, string? reason)
    {
        var task = await _context.ServiceTasks.FindAsync(id)
            ?? throw new KeyNotFoundException($"Задача {id} не найдена");

        task.Status = "Отменена";
        task.CancellationReason = reason;
        task.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
    }

    public async Task<List<CalendarEventDto>> GetCalendarAsync()
    {
        return await _context.ServiceTasks
            .Include(t => t.Machine)
            .Select(t => new CalendarEventDto
            {
                Id = t.Id,
                Title = t.Machine.Name + " - " + t.TaskType,
                Start = t.ScheduledDate,
                End = t.ScheduledDate.AddHours(2),
                Color = t.Status == "Закрыта" ? "green" : "blue"
            })
            .ToListAsync();
    }

    public async Task<int> GenerateTasksAsync()
    {
        var machines = await _context.VendingMachines.ToListAsync();
        foreach (var machine in machines)
        {
            _context.ServiceTasks.Add(new ServiceTask
            {
                MachineId = machine.Id,
                TaskType = "Плановое ТО",
                Status = "Новая",
                ScheduledDate = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            });
        }
        await _context.SaveChangesAsync();
        return machines.Count;
    }
}