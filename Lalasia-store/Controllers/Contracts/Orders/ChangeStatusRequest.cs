namespace Lalasia_store.Controllers.Contracts.Orders;

public record ChangeStatusRequest(string OrderId, string OrderStatus);