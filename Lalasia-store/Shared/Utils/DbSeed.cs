using Lalasia_store.Models;
using Lalasia_store.Models.Data;
using Lalasia_store.Models.Types;
using Microsoft.AspNetCore.Identity;

namespace Lalasia_store.Shared.Utils;

public static class DbSeed
{
    public static async Task Seed(AppDbContext dbContext, RoleManager<Role> roleManager)
    {
        await dbContext.Database.EnsureCreatedAsync();

        if (!dbContext.Products.Any())
            dbContext.Products.AddRange(SeedData.GetProducts());
        
        if (!dbContext.Users.Any())
            dbContext.Users.AddRange(SeedData.GetUsers());
        
        if (!dbContext.Carts.Any())
            dbContext.Carts.AddRange(SeedData.GetCarts());
        
        if (!dbContext.CartItems.Any())
            dbContext.CartItems.AddRange(SeedData.GetCartItems());
        
        if (!dbContext.Orders.Any())
            dbContext.Orders.AddRange(SeedData.GetOrders());
        
        UserRoles[] roles = [UserRoles.User, UserRoles.Admin];
        foreach (var role in roles)
        {
            var existingRole = await roleManager.RoleExistsAsync(role.ToString());
        
            if (!existingRole)
                await roleManager.CreateAsync(new Role(role));
        }
        
        await dbContext.SaveChangesAsync();
    }
}