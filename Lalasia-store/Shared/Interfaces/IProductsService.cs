using Lalasia_store.Controllers.Contracts.Products;
using Lalasia_store.Models.Dto;

namespace Lalasia_store.Shared.Interfaces;

public interface IProductsService
{
    public Task<GetProductsResponse> GetProducts(List<string> types, int? page, string? query);
    public Task<ProductDto> GetProduct(string id);
}