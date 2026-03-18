using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApiVending.DTO;
using Microsoft.AspNetCore.Mvc;

namespace ApiVending.Services.Companies
{
    public interface ICompanyService
    {
        Task<PaginatedResponse<CompanyDto>> GetCompaniesAsync(int page, int limit, string? searchName);
        Task<CompanyDto?> GetByIdAsync(int id);
        Task<CompanyDto> CreateAsync(CreateCompanyDto dto);
        Task UpdateAsync(int id, UpdateCompanyDto dto);
        Task DeleteAsync(int id);
        Task<List<MachineListDto>> GetCompanyMachinesAsync(int id);
        Task<CompanyStatsDto> GetCompanyStatsAsync(int id);
    }
}