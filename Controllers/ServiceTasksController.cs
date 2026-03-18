using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApiVending.DTO;
using ApiVending.Models;
using ApiVending.Services.ServerTask;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ApiVending.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    
    public class ServiceTasksController(IServerTask context) : ControllerBase
    {
         private readonly IServerTask _taskService = context;

    [HttpGet]
    public async Task<ActionResult<List<ServiceTaskDto>>> GetTasks( DateTime? fromDate, DateTime? toDate, int? machineId, int? assignedTo, string? status, string? taskType)
        {
            return Ok(await _taskService.GetTasksAsync(fromDate, toDate, machineId, assignedTo, status, taskType));
        }
        

    [HttpGet("{id}")]
    public async Task<ActionResult<ServiceTaskDto>> GetTask(int id)
    {
        var task = await _taskService.GetTaskAsync(id);
        return task == null ? NotFound() : Ok(task);
    }

    [HttpPost]
    public async Task<ActionResult<ServiceTaskDto>> CreateTask(CreateServiceTaskDto dto)
        {
            return Ok(await _taskService.CreateTaskAsync(dto));
        }
        

    [HttpPatch("{id}/start")]
    public async Task<IActionResult> StartTask(int id)
    {
        await _taskService.StartTaskAsync(id);
        return Ok();
    }

    [HttpPatch("{id}/complete")]
    public async Task<IActionResult> CompleteTask(int id)
    {
        await _taskService.CompleteTaskAsync(id);
        return Ok();
    }

    [HttpPatch("{taskId}/assign/{userId}")]
    public async Task<IActionResult> AssignTask(int taskId, int userId)
    {
        await _taskService.AssignTaskAsync(taskId, userId);
        return Ok();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteServiceTask(int id)
    {
        await _taskService.DeleteTaskAsync(id);
        return NoContent();
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateTask(int id, UpdateServiceTaskDto dto)
    {
        await _taskService.UpdateTaskAsync(id, dto);
        return Ok();
    }

    [HttpPatch("{id}/cancel")]
    public async Task<IActionResult> CancelTask(int id, [FromBody] string? reason)
    {
        await _taskService.CancelTaskAsync(id, reason);
        return Ok();
    }

    [HttpGet("calendar-tasks")]
    public async Task<ActionResult<List<CalendarEventDto>>> GetCalendar()
        {
            return Ok(await _taskService.GetCalendarAsync());
        }
        

    [HttpPost("generate")]
    public async Task<IActionResult> GenerateTasks()
        {
            return Ok(await _taskService.GenerateTasksAsync());
        }
        
    }
}