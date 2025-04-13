using System.Security.Claims;
using Lalasia_store.Contracts.Cart;
using Lalasia_store.Contracts.Common;
using Lalasia_store.Models;
using Lalasia_store.Models.Data;
using Lalasia_store.Models.Dto;
using Lalasia_store.Models.Types;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Lalasia_store.Controllers;

[ApiController]
[Route("api/[controller]/[action]")]
public class CartController : ControllerBase
{
    private readonly AppDbContext _dbContext;
    private readonly ILogger<CartController> _logger;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CartController(
        AppDbContext dbContext,
        ILogger<CartController> logger,
        IHttpContextAccessor httpContextAccessor)
    {
        _dbContext = dbContext;
        _logger = logger;
        _httpContextAccessor = httpContextAccessor;
    }

    [HttpGet]
    [Authorize]
    public async Task<IActionResult> GetCart()
    {
        try
        {
            var userId = _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId is null)
                return Unauthorized(new { error = true, message = "Не удалось распознать пользователя" });

            var cartItems = _dbContext.Carts.SelectMany(cart => cart.CartItems);
            
            var cart = await _dbContext.Carts
                .Select(cart => new CartDto() {Id = cart.Id, TotalPrice = cart.TotalPrice, CartItems = cart.CartItems, UserId = cart.UserId})
                .FirstOrDefaultAsync(cart => cart.UserId.ToString() == userId);
            
            if (cart is null)
                return NotFound(new { error = true, message = "Корзина не найдена" });

            return Ok(cart);
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "[GetCart] server error");
            return BadRequest(new { error = true, message = "Не удалось загрузить корзину" });
        }
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> AddProduct(AddProductRequest request)
    {
        try
        {
            var userId = _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId is null)
                return Unauthorized(new DefaultResponse()
                    { Error = true, Message = "Не удалось распознать пользователя" });

            var cart = await _dbContext.Carts
                .FirstOrDefaultAsync(cart => cart.UserId.ToString() == userId);

            if (cart is null)
            {
                cart = new Cart() { TotalPrice = 0, UserId = Guid.Parse(userId) };
                await _dbContext.Carts.AddAsync(cart);
                await _dbContext.SaveChangesAsync();
            }

            if (!Guid.TryParse(request.ProductId, out var productGuidId))
                return BadRequest(new DefaultResponse() { Error = true, Message = "Не удалось распознать товар" });
            
            var existingProductInCart = await _dbContext.CartItems
                .Where(cartItem => cartItem.CartId == cart.Id)
                .FirstOrDefaultAsync(cartItem => cartItem.ProductId == productGuidId);
            
            if (existingProductInCart is not null)
            {
                existingProductInCart.ProductCount += 1;
            }
            else
            {
                var newCartItem = new CartItem() { CartId = cart.Id, ProductId = productGuidId };
                await _dbContext.CartItems.AddAsync(newCartItem);
            }
            
            await _dbContext.SaveChangesAsync();
            
            return Ok(new DefaultResponse() { Error = false, Message = "Товар успешно добавлен в корзину!" });
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "[AddToCart] server error");
            return BadRequest(new DefaultResponse() { Error = true, Message = "Не удалось добавить товар" });
        }
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Update(UpdateProductRequest request)
    {
        try
        {
            if (!Guid.TryParse(request.ProductId, out var productGuidId))
                return BadRequest(new DefaultResponse() { Error = true, Message = "Не удалось распознать продукт" });

            var cartItem =
                await _dbContext.CartItems.FirstOrDefaultAsync(cartItem => cartItem.ProductId == productGuidId);

            if (cartItem is null)
                return NotFound(new DefaultResponse() { Error = true, Message = "Не удалось изменить количество" });

            cartItem.ProductCount = request.ProductCount;

            await _dbContext.SaveChangesAsync();

            return Ok(new { ProductCount = cartItem.ProductCount });
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "[UpdateCart] server error");
            return BadRequest(new DefaultResponse() { Error = true, Message = "Не удалось обновить корзину" });
        }
    }
}