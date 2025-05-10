using System.ComponentModel.DataAnnotations.Schema;
using Lalasia_store.Models.Types;

namespace Lalasia_store.Models.Data;

public class Order
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public float TotalPrice { get; set; } = 0;
    public string Name { get; set; }
    public string Phone { get; set; }
    public string Email { get; set; }
    public string Address { get; set; }
    public string Products { get; set; }
    public string? Comment { get; set; }
    public OrderStatus OrderStatus { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    
    [ForeignKey(nameof(User))]
    public Guid UserId { get; set; }
    public virtual User User { get; set; }
}