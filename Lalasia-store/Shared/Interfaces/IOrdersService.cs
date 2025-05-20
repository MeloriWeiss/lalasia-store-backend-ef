using System.Security.Claims;
using Lalasia_store.Controllers;
using Lalasia_store.Controllers.Contracts.Common;
using Lalasia_store.Controllers.Contracts.Orders;
using Lalasia_store.Models.Dto;

namespace Lalasia_store.Shared.Interfaces;

public interface IOrdersService
{
    public Task<GetOrdersResponse> GetOrders(int page, ClaimsPrincipal claimsPrincipal);
    public Task<DefaultResponse> CreateOrder(CreateOrderRequest request, ClaimsPrincipal claimsPrincipal);
    public Task<ChangeStatusResponse> ChangeOrderStatus(ChangeStatusRequest request);
    public Task<GetOrdersResponse> GetAllOrders(int page, ClaimsPrincipal claimsPrincipal);
}