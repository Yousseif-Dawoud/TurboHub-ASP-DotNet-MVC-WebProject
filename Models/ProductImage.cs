
namespace TurboHub.Models;

// This class represents an image associated with a product.
public class ProductImage
{
    public int Id { get; set; }

    public string? ImagePath { get; set; }

    public string? ImageUrl { get; set; }

    public bool IsMain { get; set; } = false;

    // FK
    public int ProductId { get; set; }
    public Product Product { get; set; } = null!;
}