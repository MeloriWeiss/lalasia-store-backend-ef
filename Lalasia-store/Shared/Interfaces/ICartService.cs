using System.Security.Claims;
using Lalasia_store.Controllers.Contracts.Cart;
using Lalasia_store.Controllers.Contracts.Common;
using Lalasia_store.Models.Dto;
using Microsoft.AspNetCore.Mvc;

namespace Lalasia_store.Shared.Interfaces;

public interface ICartService
{
    public Task<CartDto> GetCart(ClaimsPrincipal claimsPrincipal);
    public Task<DefaultResponse> AddProduct(AddProductRequest request, ClaimsPrincipal claimsPrincipal);
    public Task UpdateProduct(UpdateProductRequest request);
    public Task RemoveProduct(string cartItemId);
}