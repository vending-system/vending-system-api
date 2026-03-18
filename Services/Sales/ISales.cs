using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApiVending.DTO;
using Microsoft.AspNetCore.Mvc;

namespace ApiVending.Services.Sales
{
    public interface ISales
    {
        Task<PaginatedResponse<SaleDto>> GetSalesAsync(int page, int limit, int? machineId,
        int? productId, int? companyId, DateTime? fromDate, DateTime? toDate);
        Task<SaleDto?> GetSaleAsync(int id);
        Task<SaleDto> CreateSaleAsync(CreateSaleDto dto);
        Task DeleteSaleAsync(int id);
        Task<List<GrafikiSalesDto>> GetSalesDynamicsAsync(int days, int? machineId, int? companyId);
        Task<List<TopProductDto>> GetTopProductsAsync(int days, int limit, int? machineId, int? companyId);
        Task<SalesSummaryDto> GetSalesSummaryAsync(int? companyId, int? machineId);
    }
}