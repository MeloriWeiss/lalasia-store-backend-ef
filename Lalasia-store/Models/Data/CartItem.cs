using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Lalasia_store.Models.Data;

public class CartItem
{
    [Key] public Guid Id { get; set; } = Guid.NewGuid();
    public int ProductCount { get; set; } = 1;
    
    [ForeignKey(nameof(Product))]
    public Guid ProductId { get; set; }
    public virtual Product Product { get; set; }
    
    [ForeignKey(nameof(Cart))]
    public Guid CartId { get; set; }
    public virtual Cart Cart { get; set; }
}