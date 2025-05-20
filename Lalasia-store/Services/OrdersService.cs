using System.Security.Claims;
using Lalasia_store.Controllers.Contracts.Common;
using Lalasia_store.Controllers.Contracts.Orders;
using Lalasia_store.Models;
using Lalasia_store.Models.Data;
using Lalasia_store.Models.Dto;
using Lalasia_store.Models.Types;
using Lalasia_store.Shared.Exceptions;
using Lalasia_store.Shared.Interfaces;
using Lalasia_store.Shared.Utils;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Lalasia_store.Services;

public class OrdersService : IOrdersService
{
    private readonly AppDbContext _dbContext;
    private readonly UserManager<User> _userManager;

    public OrdersService(
        AppDbContext dbContext,
        UserManager<User> userManager)
    {
        _dbContext = dbContext;
        _userManager = userManager;
    }

    public async Task<GetOrdersResponse> GetOrders(int page, ClaimsPrincipal claimsPrincipal)
    {
        var user = await _userManager.GetUserAsync(claimsPrincipal);

        if (user is null)
            throw new NotFoundException("The user was not found");

        var orders = _dbContext.Orders
            .Where(order => order.UserId == user.Id)
            .Select(order => new OrderDto()
            {
                Id = order.Id, TotalPrice = order.TotalPrice, Name = order.Name, Phone = order.Phone,
                Email = order.Email, Address = order.Address, Products = order.Products, Comment = order.Comment,
                OrderStatus = order.OrderStatus.ToString(), CreatedAt = order.CreatedAt
            })
            .OrderByDescending(order => order.CreatedAt);

        var totalOrdersCount = orders.Count();

        var returnOrders = await orders
            .Skip((page - 1) * 10)
            .Take(10)
            .ToListAsync();

        return new GetOrdersResponse() { OrdersCount = totalOrdersCount, Orders = returnOrders };
    }

    public async Task<DefaultResponse> CreateOrder(CreateOrderRequest request, ClaimsPrincipal claimsPrincipal)
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

        if (cart is null)
            throw new NotFoundException("The cart was not found");

        if (!cart.CartItems.Any())
            throw new BadRequestException("The cart is empty");

        var order = new Order()
        {
            Name = request.Name,
            Phone = request.Phone,
            Email = request.Email,
            Address = request.Address,
            TotalPrice = cart.TotalPrice,
            Products = ProductsUtil.CartToString(cart.CartItems),
            UserId = user.Id
        };

        if (request.Comment is not null && request.Comment != string.Empty)
            order.Comment = request.Comment;

        await _dbContext.Orders.AddAsync(order);

        var cartItems = _dbContext.CartItems
            .Where(cartItem => cartItem.CartId == cart.Id);

        _dbContext.CartItems.RemoveRange(cartItems);
        cart.TotalPrice = 0;

        await _dbContext.SaveChangesAsync();

        return new DefaultResponse() { Error = false, Message = "The order was successfully created" };
    }

    public async Task<ChangeStatusResponse> ChangeOrderStatus(ChangeStatusRequest request)
    {
        if (!Guid.TryParse(request.OrderId, out var orderId))
            throw new BadRequestException("The order couldn't be recognized");

        var order = await _dbContext.Orders.FindAsync(orderId);

        if (order is null)
            throw new NotFoundException("The order was not found");

        if (!Enum.TryParse<OrderStatus>(request.OrderStatus, out var orderStatus))
            throw new BadRequestException("Incorrect order status");

        order.OrderStatus = orderStatus;

        await _dbContext.SaveChangesAsync();

        return new ChangeStatusResponse() { NewStatus = orderStatus.ToString() };
    }

    public async Task<GetOrdersResponse> GetAllOrders(int page, ClaimsPrincipal claimsPrincipal)
    {
        var orders = _dbContext.Orders
            .Select(order => new OrderDto()
            {
                Id = order.Id, TotalPrice = order.TotalPrice, Name = order.Name, Phone = order.Phone,
                Email = order.Email, Address = order.Address, Products = order.Products, Comment = order.Comment,
                OrderStatus = order.OrderStatus.ToString(), CreatedAt = order.CreatedAt
            })
            .OrderByDescending(order => order.CreatedAt);

        var totalOrdersCount = orders.Count();

        var returnOrders = await orders
            .Skip((page - 1) * 10)
            .Take(10)
            .ToListAsync();

        return new GetOrdersResponse() { OrdersCount = totalOrdersCount, Orders = returnOrders };
    }
}