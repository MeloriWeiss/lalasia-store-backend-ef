namespace Lalasia_store.Models.Dto;

public class ProductDto
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Type { get; set; }
    public string ImageUrl { get; set; }
    public float Price { get; set; }
    public string? Description { get; set; }
}