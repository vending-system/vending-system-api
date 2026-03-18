using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApiVending.DTO;
using ApiVending.Models;
using Microsoft.EntityFrameworkCore;

namespace ApiVending.Services.Sales
{
    public class SalesService(VendingSystemDbContext context) : ISales
{
    private readonly VendingSystemDbContext _context = context;

    private static string GetDayName(DateOnly date)
    {
        var days = new Dictionary<DayOfWeek, string>
        {
            { DayOfWeek.Monday, "Понедельник" },
            { DayOfWeek.Tuesday, "Вторник" },
            { DayOfWeek.Wednesday, "Среда" },
            { DayOfWeek.Thursday, "Четверг" },
            { DayOfWeek.Friday, "Пятница" },
            { DayOfWeek.Saturday, "Суббота" },
            { DayOfWeek.Sunday, "Воскресенье" }
        };
        return days[date.DayOfWeek];
    }

    public async Task<PaginatedResponse<SaleDto>> GetSalesAsync(int page, int limit,
        int? machineId, int? productId, int? companyId, DateTime? fromDate, DateTime? toDate)
    {
        var query = _context.Sales
            .Include(s => s.Machine)
            .Include(s => s.Product)
            .Include(s => s.PaymentType)
            .AsQueryable();

        if (machineId.HasValue) query = query.Where(s => s.MachineId == machineId);
        if (productId.HasValue) query = query.Where(s => s.ProductId == productId);
        if (companyId.HasValue) query = query.Where(s => s.Machine != null && s.Machine.CompanyId == companyId);
        if (fromDate.HasValue) query = query.Where(s => s.SaleDate >= fromDate.Value);
        if (toDate.HasValue) query = query.Where(s => s.SaleDate <= toDate.Value);

        var total = await query.CountAsync();

        var sales = await query
            .OrderBy(s=>s.Id)
            .Skip((page - 1) * limit)
            .Take(limit)
            .Select(s => new SaleDto
            {
                Id = s.Id,
                MachineId = s.MachineId ?? 0,
                MachineName = s.Machine != null ? s.Machine.Name ?? "Неизвестно" : "Неизвестно",
                ProductId = s.ProductId ?? 0,
                ProductName = s.Product != null ? s.Product.Name : "Неизвестно",
                Quantity = s.Quantity,
                Amount = s.Amount,
                PaymentType = s.PaymentType != null ? s.PaymentType.Name : "Неизвестно",
                SaleDate = s.SaleDate
            })
            .ToListAsync();

        return new PaginatedResponse<SaleDto>
        {
            Data = sales,
            Total = total,
            Page = page,
            Limit = limit
        };
    }

    public async Task<SaleDto?> GetSaleAsync(int id)
    {
        return await _context.Sales
            .Include(s => s.Machine)
            .Include(s => s.Product)
            .Include(s => s.PaymentType)
            .Where(s => s.Id == id)
            .Select(s => new SaleDto
            {
                Id = s.Id,
                MachineId = s.MachineId ?? 0,
                MachineName = s.Machine != null ? s.Machine.Name ?? "Неизвестно" : "Неизвестно",
                ProductId = s.ProductId ?? 0,
                ProductName = s.Product != null ? s.Product.Name : "Неизвестно",
                Quantity = s.Quantity,
                Amount = s.Amount,
                PaymentType = s.PaymentType != null ? s.PaymentType.Name : "Неизвестно",
                SaleDate = s.SaleDate
            })
            .FirstOrDefaultAsync();
    }

    public async Task<SaleDto> CreateSaleAsync(CreateSaleDto dto)
    {
        var machine = await _context.VendingMachines.FindAsync(dto.MachineId)
            ?? throw new KeyNotFoundException($"Аппарат с ID {dto.MachineId} не найден");

        var product = await _context.Products.FindAsync(dto.ProductId)
            ?? throw new KeyNotFoundException($"Товар с ID {dto.ProductId} не найден");

        var inventory = await _context.MachineInventories
            .FirstOrDefaultAsync(i => i.MachineId == dto.MachineId && i.ProductId == dto.ProductId);

        if (inventory == null || inventory.Quantity < dto.Quantity)
            throw new InvalidOperationException("Недостаточно товара в аппарате");

        inventory.Quantity -= dto.Quantity;
        inventory.LastUpdated = DateTime.UtcNow;

        var sale = new Sale
        {
            MachineId = dto.MachineId,
            ProductId = dto.ProductId,
            Quantity = dto.Quantity,
            Amount = dto.Amount,
            PaymentTypeId = dto.PaymentTypeId,
            SaleDate = DateTime.UtcNow,
            CreatedAt = DateTime.UtcNow
        };

        machine.TotalRevenue = (machine.TotalRevenue ?? 0) + dto.Amount;
        machine.CurrentCash = (machine.CurrentCash ?? 0) + dto.Amount;
        machine.UpdatedAt = DateTime.UtcNow;

        _context.Sales.Add(sale);
        _context.MachineInventories.Update(inventory);
        _context.VendingMachines.Update(machine);
        await _context.SaveChangesAsync();

        var created = await _context.Sales
            .Include(s => s.Machine)
            .Include(s => s.Product)
            .Include(s => s.PaymentType)
            .FirstOrDefaultAsync(s => s.Id == sale.Id);

        return new SaleDto
        {
            Id = created!.Id,
            MachineId = created.MachineId ?? 0,
            MachineName = created.Machine?.Name ?? "Неизвестно",
            ProductId = created.ProductId ?? 0,
            ProductName = created.Product?.Name ?? "Неизвестно",
            Quantity = created.Quantity,
            Amount = created.Amount,
            PaymentType = created.PaymentType?.Name ?? "Неизвестно",
            SaleDate = created.SaleDate
        };
    }

    public async Task DeleteSaleAsync(int id)
    {
        var sale = await _context.Sales.FindAsync(id)
            ?? throw new KeyNotFoundException($"Продажа с ID {id} не найдена");

        _context.Sales.Remove(sale);
        await _context.SaveChangesAsync();
    }

    public async Task<List<GrafikiSalesDto>> GetSalesDynamicsAsync(
        int days, int? machineId, int? companyId)
    {
        var startDate = DateTime.UtcNow.AddDays(-days).Date;
        var query = _context.Sales.Include(s => s.Machine)
            .Where(s => s.SaleDate >= startDate);

        if (machineId.HasValue) query = query.Where(s => s.MachineId == machineId);
        if (companyId.HasValue) query = query.Where(s => s.Machine != null && s.Machine.CompanyId == companyId);

        var dynamics = await query
            .GroupBy(s => DateOnly.FromDateTime(s.SaleDate))
            .Select(g => new GrafikiSalesDto
            {
                Date = g.Key,
                DayName = GetDayName(g.Key),
                TotalAmount = g.Sum(s => s.Amount),
                TotalQuantity = g.Sum(s => s.Quantity),
                TransactionsCount = g.Count()
            })
            .OrderBy(d => d.Date)
            .ToListAsync();

        var result = new List<GrafikiSalesDto>();
        for (int i = 0; i < days; i++)
        {
            var date = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-(days - 1) + i));
            var existing = dynamics.FirstOrDefault(d => d.Date == date);
            result.Add(existing ?? new GrafikiSalesDto
            {
                Date = date,
                DayName = GetDayName(date),
                TotalAmount = 0,
                TotalQuantity = 0,
                TransactionsCount = 0
            });
        }
        return result;
    }

    public async Task<List<TopProductDto>> GetTopProductsAsync(
        int days, int limit, int? machineId, int? companyId)
    {
        var startDate = DateTime.UtcNow.AddDays(-days);
        var query = _context.Sales.Include(s => s.Product).Include(s => s.Machine)
            .Where(s => s.SaleDate >= startDate);

        if (machineId.HasValue) query = query.Where(s => s.MachineId == machineId);
        if (companyId.HasValue) query = query.Where(s => s.Machine != null && s.Machine.CompanyId == companyId);

        return await query
            .GroupBy(s => new { s.ProductId, s.Product.Name })
            .Select(g => new TopProductDto
            {
                ProductId = g.Key.ProductId ?? 0,
                ProductName = g.Key.Name ?? "Unknown",
                TotalQuantity = g.Sum(s => s.Quantity),
                TotalMoney = g.Sum(s => s.Amount),
                TransactionsCount = g.Count()
            })
            .OrderByDescending(x => x.TotalMoney)
            .Take(limit)
            .ToListAsync();
    }

    public async Task<SalesSummaryDto> GetSalesSummaryAsync(int? companyId, int? machineId)
    {
        var todayUtc = new DateTimeOffset(DateTime.UtcNow.Year, DateTime.UtcNow.Month,
            DateTime.UtcNow.Day, 0, 0, 0, TimeSpan.Zero);
        var tomorrowUtc = todayUtc.AddDays(1);
        var yesterdayUtc = todayUtc.AddDays(-1);

        var salesQuery = _context.Sales.AsQueryable();
        if (companyId.HasValue) salesQuery = salesQuery.Where(s => s.Machine != null && s.Machine.CompanyId == companyId);
        if (machineId.HasValue) salesQuery = salesQuery.Where(s => s.MachineId == machineId);

        var todaySales = await salesQuery.Where(s => s.SaleDate >= todayUtc.UtcDateTime && s.SaleDate < tomorrowUtc.UtcDateTime).SumAsync(s => s.Amount);
        var yesterdaySales = await salesQuery.Where(s => s.SaleDate >= yesterdayUtc.UtcDateTime && s.SaleDate < todayUtc.UtcDateTime).SumAsync(s => s.Amount);
        var totalSales = await salesQuery.SumAsync(s => s.Amount);
        var averageCheck = await salesQuery.Where(s => s.SaleDate >= todayUtc.AddDays(-30).UtcDateTime).AverageAsync(s => (double?)s.Amount) ?? 0;

        var serviceQuery = _context.ServiceTasks.AsQueryable();
        if (companyId.HasValue) serviceQuery = serviceQuery.Where(s => s.Machine != null && s.Machine.CompanyId == companyId);
        if (machineId.HasValue) serviceQuery = serviceQuery.Where(s => s.MachineId == machineId);

        var serviceToday = await serviceQuery.Where(s => s.ActualCompletionDate.HasValue &&
            s.ActualCompletionDate.Value >= todayUtc && s.ActualCompletionDate.Value < tomorrowUtc).CountAsync();
        var serviceYesterday = await serviceQuery.Where(s => s.ActualCompletionDate.HasValue &&
            s.ActualCompletionDate.Value >= yesterdayUtc && s.ActualCompletionDate.Value < todayUtc).CountAsync();

        return new SalesSummaryDto
        {
            TodaySales = todaySales,
            YesterdaySales = yesterdaySales,
            TotalSales = totalSales,
            AverageCheck = Math.Round(averageCheck, 2),
            ServiceToday = serviceToday,
            ServiceYesterday = serviceYesterday
        };
    }
}
}