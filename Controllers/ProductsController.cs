using ApiVending.DTO;
using ApiVending.Services.Product;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ProductsController(IProductService productService) : ControllerBase
{
    private readonly IProductService _productService = productService;

    [HttpGet]
    public async Task<ActionResult<PaginatedResponse<ProductDto>>> GetProducts(
        [FromQuery] int page = 1, [FromQuery] int limit = 20, [FromQuery] string? search = null)
        => Ok(await _productService.GetProductsAsync(page, limit, search));

    [HttpGet("{id}")]
    public async Task<ActionResult<ProductDto>> GetProduct(int id)
    {
        var product = await _productService.GetProductAsync(id);
        return product == null ? NotFound($"Товар с ID {id} не найден") : Ok(product);
    }

    [HttpPost]
    public async Task<ActionResult<ProductDto>> CreateProduct(CreateProductDto dto)
    {
        var result = await _productService.CreateProductAsync(dto);
        return CreatedAtAction(nameof(GetProduct), new { id = result.Id }, result);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateProduct(int id, UpdateProductDto dto)
    {
        await _productService.UpdateProductAsync(id, dto);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteProduct(int id)
    {
        await _productService.DeleteProductAsync(id);
        return NoContent();
    }

    [HttpGet("{id}/inventory")]
    public async Task<IActionResult> GetProductInventory(int id)
        => Ok(await _productService.GetProductInventoryAsync(id));

    [HttpPost("{productId}/machines/{machineId}")]
    public async Task<IActionResult> AddProductToMachine(int productId, int machineId, [FromBody] int quantity)
        => Ok(await _productService.AddProductToMachineAsync(productId, machineId, quantity));

    [HttpGet("low-stock")]
    public async Task<IActionResult> GetLowStockProducts()
        => Ok(await _productService.GetLowStockProductsAsync());

    [HttpPatch("{productId}/machines/{machineId}/quantity")]
    public async Task<IActionResult> UpdateQuantity( int productId, int machineId, [FromBody] int quantity)
    => Ok(await _productService.UpdateАmountAsync(productId, machineId, quantity));
}