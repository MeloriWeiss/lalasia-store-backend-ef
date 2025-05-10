using Lalasia_store.Models.Dto;

namespace Lalasia_store.Shared.Utils;

public static class ProductsUtil
{
    public static string CartToString(IEnumerable<CartItemDto> cartItems)
    {
        List<string> products = [];
        products.AddRange(cartItems.Select(cartItem => cartItem.Product.Name));
        
        return string.Join(", ", products);
    }
}