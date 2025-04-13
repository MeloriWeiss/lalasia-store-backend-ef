using System.ComponentModel.DataAnnotations.Schema;

namespace Lalasia_store.Models.Data;

public class Order
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public float TotalPrice { get; set; } = 0;
    public string Address { get; set; }
    public string Products { get; set; }
    
    [ForeignKey(nameof(User))]
    public Guid UserId { get; set; }
    public virtual User User { get; set; }
}