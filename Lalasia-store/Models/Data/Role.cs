using Lalasia_store.Models.Types;
using Microsoft.AspNetCore.Identity;

namespace Lalasia_store.Models.Data;

public class Role : IdentityRole<Guid>
{
    public Role() { }

    public Role(UserRoles role) : base(role.ToString()) { }
}