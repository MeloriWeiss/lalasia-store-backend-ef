using System.Text.Json;
using Lalasia_store.Models.Data;
using Lalasia_store.Models.Types;

namespace Lalasia_store.Utils;

public static class SeedData
{
    private static readonly Guid Product1Id = Guid.NewGuid();
    private static readonly Guid Product2Id = Guid.NewGuid();

    private static readonly Guid Cart1Id = Guid.NewGuid();
    private static readonly Guid Cart2Id = Guid.NewGuid();

    private static readonly Guid User1Id = Guid.NewGuid();
    private static readonly Guid User2Id = Guid.NewGuid();

    public static List<Product> GetProducts()
    {
        return new List<Product>
        {
            new Product()
            {
                Id = Product1Id,
                Name = "White Aesthetic Chair",
                Type = ProductTypes.Chair,
                Description = "Comfortable furniture for your home",
                ImageUrl = "/images/products/product-1.png",
                Price = 120.5f
            },
            new Product()
            {
                Id = Product2Id,
                Name = "Wooden Cupboard 3 Row",
                Type = ProductTypes.Cupboard,
                Description = "Combination of wood and wool",
                ImageUrl = "/images/products/product-2.png",
                Price = 77.9f
            }
        };
    }

    public static List<User> GetUsers()
    {
        return new List<User>
        {
            new User()
            {
                Id = User1Id,
                Email = "test@mail.ru",
                UserName = "Владимир",
                PhoneNumber = "89773809845",
            },
            new User()
            {
                Id = User2Id,
                Email = "test2@mail.ru",
                UserName = "Иван",
                PhoneNumber = "89454903972",
            },
        };
    }

    public static List<Cart> GetCarts()
    {
        return new List<Cart>
        {
            new Cart()
            {
                Id = Cart1Id,
                TotalPrice = 450,
                UserId = User1Id
            },
            new Cart()
            {
                Id = Cart2Id,
                TotalPrice = 380,
                UserId = User2Id
            },
        };
    }

    public static List<CartItem> GetCartItems()
    {
        return new List<CartItem>
        {
            new CartItem()
            {
                ProductCount = 2,
                ProductId = Product1Id,
                CartId = Cart1Id
            },
            new CartItem()
            {
                ProductCount = 3,
                ProductId = Product2Id,
                CartId = Cart1Id
            },
        };
    }

    public static List<Order> GetOrders()
    {
        return new List<Order>
        {
            new Order()
            {
                TotalPrice = 450,
                UserId = User1Id,
                Address = "Малая Набережная 1, подъезд 2",
                Products = JsonSerializer.Serialize(new
                { Name = "Wooden Cupboard 3 Row", Price = 92.4f, ProductCount = 2 })
            },
            new Order()
            {
                TotalPrice = 380,
                UserId = User1Id,
                Address = "Весёлые Ворота, дом 3",
                Products = JsonSerializer.Serialize(new
                    { Name = "Wooden Cupboard 3 Row", Price = 92.4f, ProductCount = 2 })
            },
        };
    }
}