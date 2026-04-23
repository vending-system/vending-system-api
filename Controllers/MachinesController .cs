using ApiVending.DTO;
using ApiVending.Services.TA;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class MachinesController(IMachineService machineService) : ControllerBase
{
    private readonly IMachineService _machineService = machineService;

    [HttpGet]
    public async Task<ActionResult<PaginatedResponse<MachineListDto>>> GetMachines( [FromQuery] int page = 1, [FromQuery] int limit = 20)
        => Ok(await _machineService.GetMachinesAsync(page, limit));

    [HttpGet("{id}")]
    public async Task<ActionResult<MachineListDto>> GetMachine(int id)
    {
        var machine = await _machineService.GetMachineAsync(id);
        return machine == null ? NotFound($"ТА с ID {id} не найден") : Ok(machine);
    }
    [HttpGet("stats")]
    public async Task<IActionResult> GetNetworkStats()
        => Ok(await _machineService.GetNetworkStatsAsync());
        
    [HttpPost]
    public async Task<ActionResult<MachineListDto>> CreateMachine(CreateMachineDto dto)
    {
        var result = await _machineService.CreateMachineAsync(dto);
        return CreatedAtAction(nameof(GetMachine), new { id = result.Id }, result);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateMachine(int id, CreateMachineDto dto)
    {
        await _machineService.UpdateMachineAsync(id, dto);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteMachine(int id)
    {
        await _machineService.DeleteMachineAsync(id);
        return NoContent();
    }

    [HttpPatch("{id}/unlink-modem")]
    public async Task<IActionResult> UnlinkModem(int id)
        => Ok(await _machineService.UnlinkModemAsync(id));

    [HttpGet("{id}/realtime")]
    public async Task<IActionResult> GetRealtimeData(int id)
        => Ok(await _machineService.GetRealtimeDataAsync(id));
}