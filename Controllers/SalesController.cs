using ApiVending.DTO;
using ApiVending.Services.Sales;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class SalesController(ISales salesService) : ControllerBase
{
    private readonly ISales _salesService = salesService;

    [HttpGet]
    public async Task<ActionResult<PaginatedResponse<SaleDto>>> GetSales( [FromQuery] int page = 1, [FromQuery] int limit = 20,[FromQuery] int? machineId = null, [FromQuery] int? productId = null,
        [FromQuery] int? companyId = null, [FromQuery] DateTime? fromDate = null,
        [FromQuery] DateTime? toDate = null)

        => Ok(await _salesService.GetSalesAsync(page, limit, machineId, productId, companyId, fromDate, toDate));

    [HttpGet("{id}")]
    public async Task<ActionResult<SaleDto>> GetSale(int id)
    {
        var sale = await _salesService.GetSaleAsync(id);
        return sale == null ? NotFound($"Продажа с ID {id} не найдена") : Ok(sale);
    }

    [HttpPost]
    public async Task<ActionResult<SaleDto>> CreateSale(CreateSaleDto dto)
    {
        var result = await _salesService.CreateSaleAsync(dto);
        return CreatedAtAction(nameof(GetSale), new { id = result.Id }, result);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteSale(int id)
    {
        await _salesService.DeleteSaleAsync(id);
        return NoContent();
    }

    [HttpGet("sales-dynamics")]
    public async Task<ActionResult<List<GrafikiSalesDto>>> GetSalesDynamics(
        [FromQuery] int days = 10, [FromQuery] int? machineId = null, [FromQuery] int? companyId = null)

        => Ok(await _salesService.GetSalesDynamicsAsync(days, machineId, companyId));

    [HttpGet("top-products")]
    public async Task<ActionResult<List<TopProductDto>>> GetTopProducts([FromQuery] int days = 30, [FromQuery] int limit = 5,
        [FromQuery] int? machineId = null, [FromQuery] int? companyId = null)

        => Ok(await _salesService.GetTopProductsAsync(days, limit, machineId, companyId));

    [HttpGet("summary")]
    public async Task<ActionResult<SalesSummaryDto>> GetSalesSummary(
        [FromQuery] int? companyId = null, [FromQuery] int? machineId = null)
        
        => Ok(await _salesService.GetSalesSummaryAsync(companyId, machineId));
}