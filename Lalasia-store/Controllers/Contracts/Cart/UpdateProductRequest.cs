namespace Lalasia_store.Controllers.Contracts.Cart;

public record UpdateProductRequest(string CartItemId, int ProductCount);