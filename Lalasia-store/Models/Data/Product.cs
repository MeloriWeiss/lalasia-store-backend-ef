using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Lalasia_store.Models.Types;

namespace Lalasia_store.Models.Data;

public class Product
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = string.Empty;
    public ProductTypes Type { get; set; }
    public string? Description { get; set; } = string.Empty;
    public string ImageUrl { get; set; } = string.Empty;
    public float Price { get; set; }
    
    [InverseProperty(nameof(CartItem.Product))]
    public virtual ICollection<CartItem> CartItems { get; set; }
}


//    [DataType(DataType.Date)]
//    public DateTime ReleaseDate { get; set; }