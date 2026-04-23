using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApiVending.DTO;

namespace ApiVending.Services.Product
{
    public interface IProductService
    {
        Task<PaginatedResponse<ProductDto>> GetProductsAsync(int page, int limit, string? search);
        Task<ProductDto?> GetProductAsync(int id);
        Task<ProductDto> CreateProductAsync(CreateProductDto dto);
        Task UpdateProductAsync(int id, UpdateProductDto dto);
        Task DeleteProductAsync(int id);
        Task<object> GetProductInventoryAsync(int id);
        Task<object> AddProductToMachineAsync(int productId, int machineId, int quantity);
        Task<object> GetLowStockProductsAsync();
        Task<object> UpdateАmountAsync(int productId, int machineId, int amount);
        
    }
}