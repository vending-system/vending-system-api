using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApiVending.DTO;

namespace ApiVending.Services.TA
{
    public interface IMachineService
    {
        Task<PaginatedResponse<MachineListDto>> GetMachinesAsync(int page, int limit);
        Task<MachineListDto?> GetMachineAsync(int id);
        Task<MachineListDto> CreateMachineAsync(CreateMachineDto dto);
        Task UpdateMachineAsync(int id, CreateMachineDto dto);
        Task DeleteMachineAsync(int id);
        Task<object> UnlinkModemAsync(int id);
        Task<object> GetRealtimeDataAsync(int id);
    }
}