using Lalasia_store.Models.Data;

namespace Lalasia_store.Models.Dto;

public class CartDto
{
    public Guid Id { get; set; }
    public float TotalPrice { get; set; }
    public Guid UserId { get; set; }
    public ICollection<CartItem> CartItems { get; set; }
}