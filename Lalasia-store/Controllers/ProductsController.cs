using Lalasia_store.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Lalasia_store.Controllers;

[ApiController]
[Route("api/[controller]/[action]")]
public class ProductsController : ControllerBase
{
    private readonly AppDbContext _dbContext;
    private readonly ILogger<ProductsController> _logger;

    public ProductsController(AppDbContext dbContext, ILogger<ProductsController> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    [HttpGet]
    public IActionResult GetProducts()
    {
        try
        {
            var products = _dbContext.Products.ToList();

            return Ok(products);
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "[GetProducts] server error");
            return BadRequest(new { error = true, message = "Не удалось загрузить товары" });
        }
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetProduct(string id)
    {
        try
        {
            if (!Guid.TryParse(id, out var guidId))
                return BadRequest(new { error = true, message = "Не удалось получить продукт" });
            
            var product = await _dbContext.Products.FindAsync(guidId);

            if (product is null)
            {
                return NotFound(new { error = true, message = "Продукт не найден" });
            }

            return Ok(product);
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "[GetProduct] server error");
            return BadRequest(new { error = true, message = "Не удалось загрузить товары" });
        }
    }
}