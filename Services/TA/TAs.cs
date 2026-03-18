using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApiVending.DTO;
using ApiVending.Models;
using Microsoft.EntityFrameworkCore;

namespace ApiVending.Services.TA
{
    public class TAs(VendingSystemDbContext context) : IMachineService
    {
         private readonly VendingSystemDbContext _context = context;

    private static MachineListDto MapToDto(VendingMachine m) => new()
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
        Company = m.Company != null ? new CompanyDto
        {
            Id = m.Company.Id,
            Name = m.Company.Name
        } : null,
        LocationAddress = m.LocationAddress,
        CommissioningDate = m.CommissioningDate,
        ModemId = m.ModemId
    };

    public async Task<PaginatedResponse<MachineListDto>> GetMachinesAsync(int page, int limit)
    {
        var total = await _context.VendingMachines.CountAsync();

        var machines = await _context.VendingMachines
            .Include(m => m.Status)
            .Include(m => m.Company)
            .OrderBy(m => m.Id)
            .Skip((page - 1) * limit)
            .Take(limit)
            .Select(m => MapToDto(m))
            .ToListAsync();

        return new PaginatedResponse<MachineListDto>
        {
            Data = machines,
            Total = total,
            Page = page,
            Limit = limit
        };
    }

    public async Task<MachineListDto?> GetMachineAsync(int id)
    {
        var machine = await _context.VendingMachines
            .Include(m => m.Status)
            .Include(m => m.Company)
            .FirstOrDefaultAsync(m => m.Id == id);

        return machine == null ? null : MapToDto(machine);
    }

    public async Task<MachineListDto> CreateMachineAsync(CreateMachineDto dto)
    {
        var serialExists = await _context.VendingMachines
            .AnyAsync(m => m.SerialNumber == dto.SerialNumber);
        if (serialExists)
            throw new InvalidOperationException("ТА с таким серийным номером уже существует");

        var inventoryExists = await _context.VendingMachines
            .AnyAsync(m => m.InventoryNumber == dto.InventoryNumber);
        if (inventoryExists)
            throw new InvalidOperationException("ТА с таким инвентарным номером уже существует");

        var manufactureDate = DateOnly.FromDateTime(dto.ManufactureDate);
        var commissioningDate = DateOnly.FromDateTime(dto.CommissioningDate);

        if (commissioningDate < manufactureDate)
            throw new ArgumentException("Дата ввода в эксплуатацию не может быть раньше даты производства");

        if (commissioningDate > DateOnly.FromDateTime(DateTime.Today))
            throw new ArgumentException("Дата ввода в эксплуатацию не может быть позже текущей даты");

        DateOnly? lastCalibrationDate = null;
        if (dto.LastCalibrationDate != null)
            lastCalibrationDate = DateOnly.FromDateTime(dto.LastCalibrationDate.Value);

        var machine = new VendingMachine
        {
            Name = dto.Name,
            SerialNumber = dto.SerialNumber,
            InventoryNumber = dto.InventoryNumber,
            ModelName = dto.ModelName,
            Manufacturer = dto.Manufacturer,
            Country = dto.Country,
            StatusId = dto.StatusId,
            CompanyId = dto.CompanyId,
            LocationAddress = dto.LocationAddress,
            CommissioningDate = commissioningDate,
            ModemId = dto.ModemId,
            ManufactureDate = manufactureDate,
            LastCalibrationDate = lastCalibrationDate,
            CalibrationIntervalMonths = dto.CalibrationIntervalMonths,
            ResourceHoursTotal = dto.ResourceHoursTotal,
            ServiceTimeHours = dto.ServiceTimeHours,
            CurrentCash = dto.CurrentCash ?? 0,
            TotalRevenue = dto.TotalRevenue ?? 0,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.VendingMachines.Add(machine);
        await _context.SaveChangesAsync();

        var created = await _context.VendingMachines
            .Include(m => m.Status)
            .Include(m => m.Company)
            .FirstOrDefaultAsync(m => m.Id == machine.Id);

        return MapToDto(created!);
    }

    public async Task UpdateMachineAsync(int id, CreateMachineDto dto)
    {
        var machine = await _context.VendingMachines.FindAsync(id)
            ?? throw new KeyNotFoundException($"Аппарат с ID {id} не найден");

        if (machine.SerialNumber != dto.SerialNumber)
        {
            var serialExists = await _context.VendingMachines
                .AnyAsync(m => m.SerialNumber == dto.SerialNumber && m.Id != id);
            if (serialExists)
                throw new InvalidOperationException("ТА с таким серийным номером уже существует");
        }

        if (machine.InventoryNumber != dto.InventoryNumber)
        {
            var inventoryExists = await _context.VendingMachines
                .AnyAsync(m => m.InventoryNumber == dto.InventoryNumber && m.Id != id);
            if (inventoryExists)
                throw new InvalidOperationException("ТА с таким инвентарным номером уже существует");
        }

        var manufactureDate = DateOnly.FromDateTime(dto.ManufactureDate);
        var commissioningDate = DateOnly.FromDateTime(dto.CommissioningDate);

        if (commissioningDate < manufactureDate)
            throw new ArgumentException("Дата ввода в эксплуатацию не может быть раньше даты производства");

        if (commissioningDate > DateOnly.FromDateTime(DateTime.Today))
            throw new ArgumentException("Дата ввода в эксплуатацию не может быть позже текущей даты");

        DateOnly? lastCalibrationDate = null;
        if (dto.LastCalibrationDate.HasValue)
            lastCalibrationDate = DateOnly.FromDateTime(dto.LastCalibrationDate.Value);

        machine.Name = dto.Name;
        machine.SerialNumber = dto.SerialNumber;
        machine.InventoryNumber = dto.InventoryNumber;
        machine.ModelName = dto.ModelName;
        machine.Manufacturer = dto.Manufacturer;
        machine.Country = dto.Country;
        machine.StatusId = dto.StatusId;
        machine.CompanyId = dto.CompanyId;
        machine.LocationAddress = dto.LocationAddress;
        machine.CommissioningDate = commissioningDate;
        machine.ModemId = dto.ModemId;
        machine.ManufactureDate = manufactureDate;
        machine.LastCalibrationDate = lastCalibrationDate;
        machine.CalibrationIntervalMonths = dto.CalibrationIntervalMonths;
        machine.ResourceHoursTotal = dto.ResourceHoursTotal;
        machine.ServiceTimeHours = dto.ServiceTimeHours;
        machine.CurrentCash = dto.CurrentCash ?? 0;
        machine.TotalRevenue = dto.TotalRevenue ?? 0;
        machine.UpdatedAt = DateTime.UtcNow;

        _context.VendingMachines.Update(machine);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteMachineAsync(int id)
    {
        var machine = await _context.VendingMachines.FindAsync(id)
            ?? throw new KeyNotFoundException($"Аппарат с ID {id} не найден");

        _context.VendingMachines.Remove(machine);
        await _context.SaveChangesAsync();
    }

    public async Task<object> UnlinkModemAsync(int id)
    {
        var machine = await _context.VendingMachines.FindAsync(id)
            ?? throw new KeyNotFoundException($"Аппарат с ID {id} не найден");

        machine.ModemId = null;
        machine.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        return new { message = "Модем отвязан", modemId = machine.ModemId };
    }

    public async Task<object> GetRealtimeDataAsync(int id)
    {
        var machine = await _context.VendingMachines
            .Include(m => m.Status)
            .FirstOrDefaultAsync(m => m.Id == id)
            ?? throw new KeyNotFoundException($"Аппарат с ID {id} не найден");

        var random = new Random();

        return new
        {
            connection = new
            {
                status = random.Next(0, 10) > 2 ? "online" : "offline",
                provider = machine.ModemId != null ? "MegaFon" : "Нет связи",
                signal = random.Next(40, 100),
                lastSeen = DateTime.Now.AddMinutes(-random.Next(1, 30))
            },
            load = new
            {
                coffee = random.Next(0, 100),
                sugar = random.Next(0, 100),
                milk = random.Next(0, 100),
                cups = random.Next(0, 100),
                overall = random.Next(30, 95)
            },
            cash = new
            {
                fullMoney = machine.CurrentCash ?? 0,
                coins = random.Next(100, 1000),
                bills = random.Next(400, 4000),
                changeAvailable = random.Next(0, 2) == 1
            },
            statuses = new[]
            {
                new { code = "ok", message = "Работает в штатном режиме", type = "info" },
                new { code = random.Next(0, 2) == 1 ? "low_stock" : "normal",
                      message = random.Next(0, 2) == 1 ? "Заканчивается кофе" : "Все в норме",
                      type = random.Next(0, 2) == 1 ? "warning" : "info" }
            },
            events = new[]
            {
                new { time = DateTime.Now.AddMinutes(-15).ToString("HH:mm"),
                      type = "sale", product = "Латте", amount = 150 },
                new { time = DateTime.Now.AddMinutes(-5).ToString("HH:mm"),
                      type = "sale", product = "Капучино", amount = 130 }
            }
        };
    }
    }
}