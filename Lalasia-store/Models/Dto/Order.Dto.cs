using Lalasia_store.Models.Types;

namespace Lalasia_store.Models.Dto;

public class OrderDto
{
    public Guid Id { get; set; }
    public float TotalPrice { get; set; }
    public string Name { get; set; }
    public string Phone { get; set; }
    public string Email { get; set; }
    public string Address { get; set; }
    public string Products { get; set; }
    public string? Comment { get; set; }
    public string OrderStatus { get; set; }
    
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}