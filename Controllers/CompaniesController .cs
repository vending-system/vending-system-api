using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApiVending.DTO;
using ApiVending.Models;
using ApiVending.Services.Companies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ApiVending.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]

    public class CompaniesController(ICompanyService company) : ControllerBase
    {
        private readonly ICompanyService _companyService = company;
    
    [HttpGet]
    public async Task<ActionResult<PaginatedResponse<CompanyDto>>> GetCompanies([FromQuery] int page = 1,
    [FromQuery] int limit = 20,[FromQuery] string? searchName = null)
    {
        return Ok(await _companyService.GetCompaniesAsync(page, limit, searchName));
    }

        [HttpGet("{id}")]
        public async Task<ActionResult<CompanyDto>> GetCompany(int id)
        {
            var company = await _companyService.GetByIdAsync(id);

            if (company == null)
                return NotFound($"Компания с ID {id} не найдена");

            return Ok(company);
        }

         [HttpPost]
        public async Task<ActionResult<CompanyDto>> CreateCompany(CreateCompanyDto createDto)
        {        
            var result = await _companyService.CreateAsync(createDto);
            return CreatedAtAction(nameof(GetCompany), new { id = result.Id }, result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCompany(int id, UpdateCompanyDto updateDto)
        {
            await _companyService.UpdateAsync(id,updateDto);
            return NoContent();
        }

         [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCompany(int id)
        {
            await _companyService.DeleteAsync(id);
            return NoContent();
        }
        // все та компании
        [HttpGet("{id}/machines")]
        public async Task<ActionResult<List<MachineListDto>>> GetCompanyMachines(int id)
        {
            var machines = await _companyService.GetCompanyMachinesAsync(id);
            return Ok(machines);
        }
        [HttpGet("{id}/stats")]
        public async Task<IActionResult> GetCompanyStats(int id)
        {
            var stats = await _companyService.GetCompanyStatsAsync(id);

            return Ok(stats);
        }
    }
}