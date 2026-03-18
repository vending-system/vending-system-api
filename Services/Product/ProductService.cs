using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApiVending.DTO;
using ApiVending.Models;
using Microsoft.EntityFrameworkCore;

namespace ApiVending.Services.Product
{
   public class ProductService(VendingSystemDbContext context) : IProductService
{
    private readonly VendingSystemDbContext _context = context;

    public async Task<PaginatedResponse<ProductDto>> GetProductsAsync(int page, int limit, string? search)
    {
        var query = _context.Products.AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
            query = query.Where(p => p.Name.Contains(search) ||
                (p.Description != null && p.Description.Contains(search)));

        var total = await query.CountAsync();
        var products = await query
            .Skip((page - 1) * limit)
            .Take(limit)
            .OrderBy(p => p.Id)
            .Select(p => new ProductDto
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                Price = p.Price,
                MinStockLevel = p.MinStockLevel,
                AvgDailySales = p.AvgDailySales
            })
            .ToListAsync();

        return new PaginatedResponse<ProductDto>
        {
            Data = products,
            Total = total,
            Page = page,
            Limit = limit
        };
    }

    public async Task<ProductDto?> GetProductAsync(int id)
    {
        return await _context.Products
            .Where(p => p.Id == id)
            .Select(p => new ProductDto
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                Price = p.Price,
                MinStockLevel = p.MinStockLevel,
                AvgDailySales = p.AvgDailySales
            })
            .FirstOrDefaultAsync();
    }

    public async Task<ProductDto> CreateProductAsync(CreateProductDto dto)
    {
        var product = new ApiVending.Models.Product
        {
            Name = dto.Name,
            Description = dto.Description,
            Price = dto.Price,
            MinStockLevel = dto.MinStockLevel,
            AvgDailySales = dto.AvgDailySales,
            CreatedAt = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Unspecified)
        };

        _context.Products.Add(product);
        await _context.SaveChangesAsync();

        return new ProductDto
        {
            Id = product.Id,
            Name = product.Name,
            Description = product.Description,
            Price = product.Price,
            MinStockLevel = product.MinStockLevel,
            AvgDailySales = product.AvgDailySales
        };
    }

    public async Task UpdateProductAsync(int id, UpdateProductDto dto)
    {
        var product = await _context.Products.FindAsync(id)
            ?? throw new KeyNotFoundException($"Товар с ID {id} не найден");

        product.Name = dto.Name;
        product.Description = dto.Description;
        product.Price = dto.Price;
        product.MinStockLevel = dto.MinStockLevel;
        product.AvgDailySales = dto.AvgDailySales;

        _context.Products.Update(product);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteProductAsync(int id)
    {
        var product = await _context.Products.FindAsync(id)
            ?? throw new KeyNotFoundException($"Товар с ID {id} не найден");

        var hasInventory = await _context.MachineInventories.AnyAsync(i => i.ProductId == id);
        if (hasInventory)
            throw new InvalidOperationException("Нельзя удалить товар, который есть в аппаратах. Сначала удалите остатки.");

        _context.Products.Remove(product);
        await _context.SaveChangesAsync();
    }

    public async Task<object> GetProductInventoryAsync(int id)
    {
        var product = await _context.Products.FindAsync(id)
            ?? throw new KeyNotFoundException($"Товар с ID {id} не найден");

        var inventory = await _context.MachineInventories
            .Include(i => i.Machine)
            .Where(i => i.ProductId == id)
            .Select(i => new
            {
                i.MachineId,
                MachineName = i.Machine != null ? i.Machine.Name : "Неизвестно",
                LocationAddress = i.Machine != null ? i.Machine.LocationAddress : "",
                i.Quantity,
                NeedRestock = i.Quantity <= product.MinStockLevel
            })
            .ToListAsync();

        return new
        {
            ProductId = id,
            ProductName = product.Name,
            TotalQuantity = inventory.Sum(i => i.Quantity),
            Machines = inventory
        };
    }

    public async Task<object> AddProductToMachineAsync(int productId, int machineId, int quantity)
    {
        var product = await _context.Products.FindAsync(productId)
            ?? throw new KeyNotFoundException($"Товар с ID {productId} не найден");

        _ = await _context.VendingMachines.FindAsync(machineId)
            ?? throw new KeyNotFoundException($"Аппарат с ID {machineId} не найден");

        var inventory = await _context.MachineInventories
            .FirstOrDefaultAsync(i => i.MachineId == machineId && i.ProductId == productId);

        if (inventory == null)
        {
            inventory = new MachineInventory
            {
                MachineId = machineId,
                ProductId = productId,
                Quantity = quantity,
                LastUpdated = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Unspecified)
            };
            _context.MachineInventories.Add(inventory);
        }
        else
        {
            inventory.Quantity += quantity;
            inventory.LastUpdated = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Unspecified);
            _context.MachineInventories.Update(inventory);
        }

        await _context.SaveChangesAsync();
        return new { message = "Товар добавлен", quantity = inventory.Quantity };
    }

    public async Task<object> GetLowStockProductsAsync()
    {
        return await _context.MachineInventories
            .Include(i => i.Product)
            .Include(i => i.Machine)
            .Where(i => i.Quantity <= i.Product.MinStockLevel)
            .Select(i => new
            {
                i.MachineId,
                MachineName = i.Machine != null ? i.Machine.Name : "",
                i.ProductId,
                ProductName = i.Product != null ? i.Product.Name : "",
                CurrentQuantity = i.Quantity,
                i.Product.MinStockLevel,
                NeedRestock = true
            })
            .ToListAsync();
    }
}
}