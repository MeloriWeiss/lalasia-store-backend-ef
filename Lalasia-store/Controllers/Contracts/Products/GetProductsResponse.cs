using Lalasia_store.Models.Dto;

namespace Lalasia_store.Controllers.Contracts.Products;

public class GetProductsResponse()
{
    public int ProductsCount { get; set; }
    public List<ProductDto> Products { get; set; }
}