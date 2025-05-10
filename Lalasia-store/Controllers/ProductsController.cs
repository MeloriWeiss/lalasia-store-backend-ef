using Lalasia_store.Controllers.Contracts.Common;
using Lalasia_store.Models;
using Lalasia_store.Shared.Exceptions;
using Lalasia_store.Shared.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Lalasia_store.Controllers;

[ApiController]
[Route("api/[controller]/[action]")]
public class ProductsController : ControllerBase
{
    private readonly IProductsService _productsService;
    private readonly ILogger<ProductsController> _logger;

    public ProductsController(
        IProductsService productsService,
        ILogger<ProductsController> logger)
    {
        _productsService = productsService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> GetProducts([FromQuery] List<string> types, [FromQuery] int? page, [FromQuery] string? query)
    {
        try
        {
            var result = await _productsService.GetProducts(types, page, query);

            return Ok(result);
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "[GetProducts] server error");
            return BadRequest(new DefaultResponse() { Error = true, Message = "Couldn't get the products" });
        }
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetProduct(string id)
    {
        try
        {
            var product = await _productsService.GetProduct(id);

            return Ok(product);
        }
        catch (NotFoundException notFoundException)
        {
            _logger.LogError(notFoundException, "[GetProduct] server error");
            return NotFound(new DefaultResponse() { Error = true, Message = notFoundException.Message });
        }
        catch (BadRequestException badRequestException)
        {
            _logger.LogError(badRequestException, "[GetProduct] server error");
            return BadRequest(new DefaultResponse() { Error = true, Message = badRequestException.Message });
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "[GetProduct] server error");
            return BadRequest(new DefaultResponse() { Error = true, Message = "Couldn't get the product" });
        }
    }
}