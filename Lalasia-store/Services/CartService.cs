using System.Security.Claims;
using Lalasia_store.Controllers.Contracts.Cart;
using Lalasia_store.Controllers.Contracts.Common;
using Lalasia_store.Models;
using Lalasia_store.Models.Data;
using Lalasia_store.Models.Dto;
using Lalasia_store.Shared.Exceptions;
using Lalasia_store.Shared.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Lalasia_store.Services;

public class CartService : ICartService
{
    private readonly AppDbContext _dbContext;
    private readonly UserManager<User> _userManager;

    public CartService(
        AppDbContext dbContext,
        UserManager<User> userManager)
    {
        _dbContext = dbContext;
        _userManager = userManager;
    }

    public async Task<CartDto> GetCart(ClaimsPrincipal claimsPrincipal)
    {
        var user = await _userManager.GetUserAsync(claimsPrincipal);

        if (user is null)
            throw new NotFoundException("The user was not found");

        var cart = await _dbContext.Carts
            .Select(cart => new CartDto()
            {
                Id = cart.Id,
                TotalPrice = cart.TotalPrice,
                CartItems = cart.CartItems.Select(cartItem => new CartItemDto()
                {
                    Id = cartItem.Id,
                    TotalPrice = cartItem.TotalPrice,
                    ProductCount = cartItem.ProductCount,
                    Product = cartItem.Product
                }),
                UserId = cart.UserId
            })
            .FirstOrDefaultAsync(cart => cart.UserId == user.Id);

        if (cart is not null)
            return cart;

        var newCart = new Cart() { UserId = user.Id };
        await _dbContext.Carts.AddAsync(newCart);
        await _dbContext.SaveChangesAsync();

        var returnCart = new CartDto() { UserId = newCart.UserId };

        return returnCart;
    }

    public async Task<DefaultResponse> AddProduct(AddProductRequest request, ClaimsPrincipal claimsPrincipal)
    {
        var user = await _userManager.GetUserAsync(claimsPrincipal);

        if (user is null)
            throw new NotFoundException("The user was not found");

        if (!Guid.TryParse(request.ProductId, out var productGuidId))
            throw new BadRequestException("The product couldn't be recognized");

        var product = await _dbContext.Products.FindAsync(productGuidId);

        if (product is null)
            throw new NotFoundException("The product was not found");

        var cart = await _dbContext.Carts
            .Include(cart => cart.CartItems)
            .FirstOrDefaultAsync(cart => cart.UserId == user.Id);

        if (cart is null)
        {
            cart = new Cart() { TotalPrice = product.Price, UserId = user.Id };
            await _dbContext.Carts.AddAsync(cart);
            await _dbContext.SaveChangesAsync();
        }

        var existingCartItem = await _dbContext.CartItems
            .Where(cartItem => cartItem.CartId == cart.Id)
            .FirstOrDefaultAsync(cartItem => cartItem.ProductId == productGuidId);

        if (existingCartItem is not null)
        {
            existingCartItem.ProductCount += 1;
            existingCartItem.TotalPrice += product.Price;
        }
        else
        {
            var newCartItem = new CartItem()
                { CartId = cart.Id, ProductId = productGuidId, TotalPrice = product.Price };
            await _dbContext.CartItems.AddAsync(newCartItem);
        }

        cart.TotalPrice = cart.CartItems.Sum(item => item.TotalPrice);

        await _dbContext.SaveChangesAsync();

        return new DefaultResponse() { Error = false, Message = "The product was successfully added to the cart" };
    }

    public async Task UpdateProduct(UpdateProductRequest request)
    {
        if (!Guid.TryParse(request.CartItemId, out var cartGuidId))
            throw new BadRequestException("The product couldn't be recognized");

        var cartItem =
            await _dbContext.CartItems.FindAsync(cartGuidId);

        if (cartItem is null)
            throw new NotFoundException("The item of cart was not found");

        cartItem.ProductCount = request.ProductCount;

        var product = await _dbContext.Products.FindAsync(cartItem.ProductId);

        if (product is null)
            throw new NotFoundException("The product from the cart was not found");

        cartItem.TotalPrice = request.ProductCount * product.Price;

        var cart = await _dbContext.Carts
            .Include(cart => cart.CartItems)
            .FirstOrDefaultAsync(cart => cart.Id == cartItem.CartId);

        if (cart is null)
            throw new NotFoundException("The cart was not found");

        cart.TotalPrice = cart.CartItems.Sum(item => item.TotalPrice);

        await _dbContext.SaveChangesAsync();
    }

    public async Task RemoveProduct(string cartItemId)
    {
        if (!Guid.TryParse(cartItemId, out var cartItemGuidId))
            throw new BadRequestException("The product couldn't be recognized");

        var cartItem = await _dbContext.CartItems.FindAsync(cartItemGuidId);

        if (cartItem is null)
            throw new NotFoundException("The item of cart was not found");

        _dbContext.CartItems.Remove(cartItem);
        await _dbContext.SaveChangesAsync();
    }
}