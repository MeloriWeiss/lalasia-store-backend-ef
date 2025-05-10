using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace Lalasia_store.Models.Data;

public class User : IdentityUser<Guid>
{
    public virtual Cart Cart { get; set; }

    [InverseProperty(nameof(Order.User))] public virtual ICollection<Order> Orders { get; set; }
}