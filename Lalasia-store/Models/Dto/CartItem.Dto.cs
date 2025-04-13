namespace Lalasia_store.Models.Dto;

public class CartItemDto
{
    public Guid Id { get; set; }
    public int ProductCount { get; set; }
    public Guid ProductId { get; set; }
    public Guid CartId { get; set; }
}