using Lalasia_store.Models.Data;

namespace Lalasia_store.Models.Dto;

public class CartItemDto
{
    public Guid Id { get; set; }
    public float TotalPrice { get; set; }
    public int ProductCount { get; set; }
    public Product Product { get; set; }
}