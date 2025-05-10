namespace Lalasia_store.Controllers.Contracts.Orders;

public record CreateOrderRequest(string Name, string Phone, string Email, string Address, string? Comment);