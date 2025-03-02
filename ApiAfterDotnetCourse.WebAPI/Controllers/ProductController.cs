using Microsoft.AspNetCore.Mvc;
using ApiAfterDotnetCourse.Bll.Dtos;
using ApiAfterDotnetCourse.Bll.Interfaces;
using Microsoft.AspNetCore.Authorization;

namespace ApiAfterDotnetCourse.WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Policy = "RequiredAdminRole")] // Csak Adminok férhetnek hozzá
public class ProductController : ControllerBase
{
    private readonly IProductService _productService;

    public ProductController(IProductService productService)
    {
        _productService = productService;
    }

    // Összes termék lekérdezése
    [HttpGet]
    public async Task<IActionResult> GetProducts()
    {
        var products = await _productService.GetAllProductsAsync();
        return Ok(products);
    }

    // Új termék létrehozása
    [HttpPost]
    public async Task<IActionResult> CreateProduct([FromBody] CreateProductDto dto)
    {
        if (dto == null)
        {
            return BadRequest("A termék adatai nem lehetnek üresek.");
        }

        if (string.IsNullOrWhiteSpace(dto.Name) || dto.Price <= 0)
        {
            return BadRequest("Érvénytelen termékadatok. A név nem lehet üres, és az árnak pozitívnak kell lennie.");
        }

        try
        {
            var product = await _productService.CreateProductAsync(dto);
            return CreatedAtAction(nameof(GetProducts), new { id = product.ProductId }, product);
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid("Nincs jogosultságod a művelet végrehajtásához.");
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Hiba történt a termék létrehozása közben: {ex.Message}");
        }
    }
}
