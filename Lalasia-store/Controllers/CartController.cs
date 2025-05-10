using Lalasia_store.Controllers.Contracts.Cart;
using Lalasia_store.Controllers.Contracts.Common;
using Lalasia_store.Models;
using Lalasia_store.Models.Data;
using Lalasia_store.Shared.Exceptions;
using Lalasia_store.Shared.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Lalasia_store.Controllers;

[ApiController]
[Route("api/[controller]/[action]")]
public class CartController : ControllerBase
{
    private readonly ICartService _cartService;
    private readonly ILogger<CartController> _logger;

    public CartController(
        ICartService cartService,
        ILogger<CartController> logger)
    {
        _cartService = cartService;
        _logger = logger;
    }

    [HttpGet]
    [Authorize(AuthenticationSchemes = "AccessToken")]
    public async Task<IActionResult> GetCart()
    {
        try
        {
            var cart = await _cartService.GetCart(User);

            return Ok(cart);
        }
        catch (NotFoundException notFoundException)
        {
            _logger.LogError(notFoundException, "[GetCart] server error");
            return NotFound(new DefaultResponse() { Error = true, Message = notFoundException.Message });
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "[GetCart] server error");
            return BadRequest(new DefaultResponse() { Error = true, Message = "Couldn't get the cart" });
        }
    }

    [HttpPost]
    [Authorize(AuthenticationSchemes = "AccessToken")]
    public async Task<IActionResult> AddProduct([FromBody] AddProductRequest request)
    {
        try
        {
            var result = await _cartService.AddProduct(request, User);

            return Ok(result);
        }
        catch (NotFoundException notFoundException)
        {
            _logger.LogError(notFoundException, "[GetCart] server error");
            return NotFound(new DefaultResponse() { Error = true, Message = notFoundException.Message });
        }
        catch (BadRequestException badRequestException)
        {
            _logger.LogError(badRequestException, "[GetCart] server error");
            return BadRequest(new DefaultResponse() { Error = true, Message = badRequestException.Message });
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "[AddProductToCart] server error");
            return BadRequest(new DefaultResponse() { Error = true, Message = "Couldn't add product to cart" });
        }
    }

    [HttpPatch]
    [Authorize(AuthenticationSchemes = "AccessToken")]
    public async Task<IActionResult> UpdateProduct([FromBody] UpdateProductRequest request)
    {
        try
        {
            await _cartService.UpdateProduct(request);

            return Ok();
        }
        catch (NotFoundException notFoundException)
        {
            _logger.LogError(notFoundException, "[UpdateProductInCart] server error");
            return NotFound(new DefaultResponse() { Error = true, Message = notFoundException.Message });
        }
        catch (BadRequestException badRequestException)
        {
            _logger.LogError(badRequestException, "[UpdateProductInCart] server error");
            return BadRequest(new DefaultResponse() { Error = true, Message = badRequestException.Message });
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "[UpdateProductInCart] server error");
            return BadRequest(new DefaultResponse() { Error = true, Message = "Couldn't update the cart" });
        }
    }

    [HttpDelete("{cartItemId}")]
    [Authorize(AuthenticationSchemes = "AccessToken")]
    public async Task<IActionResult> RemoveProduct(string cartItemId)
    {
        try
        {
            await _cartService.RemoveProduct(cartItemId);

            return Ok();
        }
        catch (NotFoundException notFoundException)
        {
            _logger.LogError(notFoundException, "[RemoveProductFromCart] server error");
            return NotFound(new DefaultResponse() { Error = true, Message = notFoundException.Message });
        }
        catch (BadRequestException badRequestException)
        {
            _logger.LogError(badRequestException, "[RemoveProductFromCart] server error");
            return BadRequest(new DefaultResponse() { Error = true, Message = badRequestException.Message });
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "[RemoveProductFromCart] server error");
            return BadRequest(new DefaultResponse() { Error = true, Message = "Couldn't delete the product" });
        }
    }
}