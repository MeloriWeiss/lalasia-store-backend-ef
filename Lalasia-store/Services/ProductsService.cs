using Lalasia_store.Controllers.Contracts.Products;
using Lalasia_store.Models;
using Lalasia_store.Models.Dto;
using Lalasia_store.Shared.Config;
using Lalasia_store.Shared.Exceptions;
using Lalasia_store.Shared.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace Lalasia_store.Services;

public class ProductsService : IProductsService
{
    private readonly AppDbContext _dbContext;
    private readonly IMemoryCache _memoryCache;

    public ProductsService(
        AppDbContext dbContext,
        IMemoryCache memoryCache)
    {
        _dbContext = dbContext;
        _memoryCache = memoryCache;
    }

    public async Task<GetProductsResponse> GetProducts(List<string> types, int? page, string? query)
    {
        var pageNumber = page ?? 1;
        var queryString = query?.ToLower();

        if (_memoryCache.TryGetValue(CacheKeys.Products, out List<ProductDto>? cacheProducts))
        {
            if (cacheProducts is null)
            {
                _memoryCache.Remove(CacheKeys.Products);
                throw new NotFoundException("The products was not found in the cache");
            }

            var selectedCacheProducts = cacheProducts
                .Where(product => (types.Count <= 0 || types.Contains(product.Type.ToString()))
                                  && product.Name.ToLower().Contains(queryString ?? ""))
                .ToList();

            var totalSelectedCacheProductsCount = selectedCacheProducts.Count;
            
            var returnCacheProducts = selectedCacheProducts
                .Skip((pageNumber - 1) * 6)
                .Take(pageNumber + 5)
                .ToList();

            return new GetProductsResponse()
                { ProductsCount = totalSelectedCacheProductsCount, Products = returnCacheProducts };
        }

        var products = await _dbContext.Products
            .Select(product => new ProductDto()
            {
                Id = product.Id, Name = product.Name, Type = product.Type.ToString(),
                ImageUrl = product.ImageUrl, Price = product.Price,
                Description = (product.Description ?? "").Substring(0, 100)
            })
            .ToListAsync();

        _memoryCache.Set(CacheKeys.Products, products,
            new MemoryCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromMinutes(10)));

        var selectedProducts = products
            .Where(product => (types.Count <= 0 || types.Contains(product.Type.ToString()))
                              && product.Name.ToLower().Contains(queryString ?? ""))
            .ToList();

        var totalProductsCount = selectedProducts.Count;

        var returnProducts = selectedProducts
            .Skip((pageNumber - 1) * 6)
            .Take(pageNumber + 5)
            .ToList();

        return new GetProductsResponse() { ProductsCount = totalProductsCount, Products = returnProducts };
    }

    public async Task<ProductDto> GetProduct(string id)
    {
        if (!Guid.TryParse(id, out var guidId))
            throw new BadRequestException("The product couldn't be recognized");

        if (_memoryCache.TryGetValue(CacheKeys.Products, out List<ProductDto>? cacheProducts))
        {
            if (cacheProducts is null)
            {
                _memoryCache.Remove(CacheKeys.Products);
                throw new NotFoundException("The product was not found in the cache");
            }
            
            var cacheProduct = cacheProducts.FirstOrDefault(product => product.Id == guidId);
            
            if (cacheProduct is null)
                throw new NotFoundException("The product was not found");

            var returnCacheProduct = new ProductDto()
            {
                Id = cacheProduct.Id, Name = cacheProduct.Name, Type = cacheProduct.Type.ToString(),
                ImageUrl = cacheProduct.ImageUrl, Price = cacheProduct.Price, Description = cacheProduct.Description
            };

            return returnCacheProduct;
        }
        
        var product = await _dbContext.Products.FindAsync(guidId);

        if (product is null)
            throw new NotFoundException("The product was not found");

        var returnProduct = new ProductDto()
        {
            Id = product.Id, Name = product.Name, Type = product.Type.ToString(),
            ImageUrl = product.ImageUrl, Price = product.Price, Description = product.Description
        };

        return returnProduct;
    }
}