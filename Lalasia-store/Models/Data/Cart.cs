using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Lalasia_store.Models.Data;

public class Cart
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    public float TotalPrice { get; set; } = 0;
    
    [ForeignKey(nameof(User))]
    public Guid UserId { get; set; }
    
    public virtual User User { get; set; }

    [InverseProperty(nameof(CartItem.Cart))]
    public virtual ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();
}