using Lalasia_store.Controllers.Contracts.Common;
using Lalasia_store.Controllers.Contracts.Orders;
using Lalasia_store.Models.Data;
using Lalasia_store.Models.Types;
using Lalasia_store.Shared.Exceptions;
using Lalasia_store.Shared.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Lalasia_store.Controllers;

[ApiController]
[Route("api/[controller]/[action]")]
public class OrdersController : ControllerBase
{
    private readonly IOrdersService _ordersService;
    private readonly ILogger<OrdersController> _logger;

    public OrdersController(
        IOrdersService ordersService,
        ILogger<OrdersController> logger)
    {
        _ordersService = ordersService;
        _logger = logger;
    }

    [HttpGet]
    [Authorize(AuthenticationSchemes = "AccessToken")]
    public async Task<IActionResult> GetOrders([FromQuery] int page)
    {
        try
        {
            var result = await _ordersService.GetOrders(page, User);

            return Ok(result);
        }
        catch (NotFoundException notFoundException)
        {
            _logger.LogError(notFoundException, "[GetOrders] server error");
            return NotFound(new DefaultResponse() { Error = true, Message = notFoundException.Message });
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "[GetOrders] server error");
            return BadRequest(new DefaultResponse() { Error = true, Message = "Couldn't get the orders" });
        }
    }

    [HttpPost]
    [Authorize(AuthenticationSchemes = "AccessToken")]
    public async Task<IActionResult> CreateOrder([FromBody] CreateOrderRequest request)
    {
        try
        {
            var result = await _ordersService.CreateOrder(request, User);

            return Ok(result);
        }
        catch (NotFoundException notFoundException)
        {
            _logger.LogError(notFoundException, "[CreateOrder] server error");
            return NotFound(new DefaultResponse() { Error = true, Message = notFoundException.Message });
        }
        catch (BadRequestException badRequestException)
        {
            _logger.LogError(badRequestException, "[CreateOrder] server error");
            return BadRequest(new DefaultResponse() { Error = true, Message = badRequestException.Message });
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "[CreateOrder] server error");
            return BadRequest(new DefaultResponse() { Error = true, Message = "Couldn't create an order" });
        }
    }

    [HttpPatch]
    [Authorize(AuthenticationSchemes = "AccessToken", Roles = "Admin")]
    public async Task<IActionResult> ChangeOrderStatus([FromBody] ChangeStatusRequest request)
    {
        try
        {
            var result = await _ordersService.ChangeOrderStatus(request);

            return Ok(result);
        }
        catch (NotFoundException notFoundException)
        {
            _logger.LogError(notFoundException, "[ChangeOrderStatus] server error");
            return NotFound(new DefaultResponse() { Error = true, Message = notFoundException.Message });
        }
        catch (BadRequestException badRequestException)
        {
            _logger.LogError(badRequestException, "[ChangeOrderStatus] server error");
            return BadRequest(new DefaultResponse() { Error = true, Message = badRequestException.Message });
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "[ChangeOrderStatus] server error");
            return BadRequest(new DefaultResponse() { Error = true, Message = "Couldn't change the order status" });
        }
    }
}