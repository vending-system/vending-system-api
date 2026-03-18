using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApiVending.DTO;
using ApiVending.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ApiVending.Services.Companies
{
    public class CompanyService(VendingSystemDbContext context) : ICompanyService
    {
        private readonly VendingSystemDbContext _context = context;
        async public Task<CompanyDto> CreateAsync(CreateCompanyDto dto)
        {
            
            if (!string.IsNullOrWhiteSpace(dto.Inn))
            {
                var innExists = await _context.Companies.AnyAsync(c => c.Inn == dto.Inn);
                if (innExists)
                    throw new InvalidOperationException("Компания с таким ИНН уже существует");
            }
            var company = new Company
            {
                Name = dto.Name,
                Inn = dto.Inn,
                Address = dto.Address
            };

            _context.Companies.Add(company);
            await _context.SaveChangesAsync();

            return  new CompanyDto
            {
                Id = company.Id,
                Name = company.Name,
                Inn = company.Inn,
                Address = company.Address
            };
            
        }

        async public Task DeleteAsync(int id)
        {
            var company = await _context.Companies.FindAsync(id) ?? throw new KeyNotFoundException($"Компания с ID {id} не найдена");
            var hasMachines = await _context.VendingMachines.AnyAsync(m => m.CompanyId == id);
            if (hasMachines)
            throw new InvalidOperationException("Нельзя удалить компанию, у которой есть торговые аппараты");

            _context.Companies.Remove(company);
            await _context.SaveChangesAsync();

        }

        public async Task<CompanyDto?> GetByIdAsync(int id)
        {
            return await _context.Companies.Where(c => c.Id == id)
            .Select(c => new CompanyDto
            {
                Id = c.Id,
                Name = c.Name,
                Inn = c.Inn,
                Address = c.Address
            }).FirstOrDefaultAsync();
        }

         public async Task<PaginatedResponse<CompanyDto>> GetCompaniesAsync(
            int page, int limit, string? searchName)
        {
            var query = _context.Companies.AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchName))
                query = query.Where(c => c.Name.Contains(searchName) ||(c.Inn != null && c.Inn.Contains(searchName)));

            var total = await query.CountAsync();
            var companies = await query
                .OrderBy(c => c.Name)
                .Skip((page - 1) * limit)
                .Take(limit)
                .Select(c => new CompanyDto
                {
                    Id = c.Id,
                    Name = c.Name,
                    Inn = c.Inn,
                    Address = c.Address
                })
                .ToListAsync();

            return new PaginatedResponse<CompanyDto>
            {
                Data = companies,
                Total = total,
                Page = page,
                Limit = limit
            };
        }

        async public Task<List<MachineListDto>> GetCompanyMachinesAsync(int id)
        {
            var company = await _context.Companies.FindAsync(id) ?? throw new KeyNotFoundException($"Компания с ID {id} не найдена");
            return await _context.VendingMachines.Include(m => m.Status).Where(m => m.CompanyId == id)
                .Select(m => new MachineListDto
                {
                    Id = m.Id,
                    Name = m.Name,
                    SerialNumber = m.SerialNumber,
                    ModelName = m.ModelName,
                    Status = m.Status != null ? new MachineStatusDto
                    {
                        Id = m.Status.Id,
                        Name = m.Status.Name
                    } : null,
                    LocationAddress = m.LocationAddress,
                    CommissioningDate = m.CommissioningDate,
                    ModemId = m.ModemId
                })
                .ToListAsync();            
        }

         async public Task<CompanyStatsDto> GetCompanyStatsAsync(int id)
        {
            var company = await _context.Companies.FindAsync(id) ?? throw new KeyNotFoundException($"Компания с ID {id} не найдена");
            var machines = await _context.VendingMachines.Where(m => m.CompanyId == id).ToListAsync();

            return new CompanyStatsDto
            {
                TotalMachines = machines.Count,
                WorkingMachines = machines.Count(m => m.StatusId == 1),
                BrokenMachines = machines.Count(m => m.StatusId == 2),
                MaintenanceMachines = machines.Count(m => m.StatusId == 3),
                TotalRevenue = machines.Sum(m => m.TotalRevenue ?? 0),
                TotalCash = machines.Sum(m => m.CurrentCash ?? 0)
            };
        }

        async public Task UpdateAsync(int id, UpdateCompanyDto dto)
        {
            var company = await _context.Companies.FindAsync(id) ?? throw new KeyNotFoundException($"Компания с ID {id} не найдена");
    

            if (!string.IsNullOrWhiteSpace(dto.Inn) && dto.Inn != company.Inn)
            {
                var innExists = await _context.Companies.AnyAsync(c => c.Inn == dto.Inn && c.Id != id);
        
                if (innExists) throw new InvalidOperationException("Компания с таким ИНН уже существует");
            }

            company.Name = dto.Name;
            company.Inn = dto.Inn;
            company.Address = dto.Address;

            _context.Companies.Update(company);
            await _context.SaveChangesAsync();
        }
    }
}