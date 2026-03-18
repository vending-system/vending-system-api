using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApiVending.DTO;
using ApiVending.Models;
using Microsoft.AspNetCore.Mvc;

namespace ApiVending.Services.ServerTask
{
    public interface IServerTask
    {
        Task<List<ServiceTaskDto>> GetTasksAsync(DateTime? fromDate, DateTime? toDate, 
        int? machineId, int? assignedTo, string? status, string? taskType);
        Task<ServiceTaskDto?> GetTaskAsync(int id);
        Task<ServiceTaskDto> CreateTaskAsync(CreateServiceTaskDto dto);
        Task StartTaskAsync(int id);
        Task CompleteTaskAsync(int id);
        Task AssignTaskAsync(int taskId, int userId);
        Task DeleteTaskAsync(int id);
        Task UpdateTaskAsync(int id, UpdateServiceTaskDto dto);
        Task CancelTaskAsync(int id, string? reason);
        Task<List<CalendarEventDto>> GetCalendarAsync();
        Task<int> GenerateTasksAsync();
        
    }
}