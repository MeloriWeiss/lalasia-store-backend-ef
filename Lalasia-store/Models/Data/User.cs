using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Lalasia_store.Models.Types;
using Microsoft.AspNetCore.Identity;

namespace Lalasia_store.Models.Data;

public class User : IdentityUser<Guid>
{
    public virtual RefreshToken RefreshToken { get; set; }

    public virtual Cart Cart { get; set; }

    [InverseProperty(nameof(Order.User))] public virtual ICollection<Order> Orders { get; set; }
}

public class Role : IdentityRole<Guid>
{
    public Role() { }

    public Role(UserRoles role) : base(role.ToString()) { }
}