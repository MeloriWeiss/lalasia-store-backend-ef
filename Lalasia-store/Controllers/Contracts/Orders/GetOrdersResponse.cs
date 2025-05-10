using Lalasia_store.Models.Dto;

namespace Lalasia_store.Controllers.Contracts.Orders;

public class GetOrdersResponse
{
    public int OrdersCount { get; set; }
    public List<OrderDto> Orders { get; set; }
}